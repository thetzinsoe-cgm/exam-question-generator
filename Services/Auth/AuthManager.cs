using ExamSystem.Constraints;
using ExamSystem.DAO.AdminUser;
using ExamSystem.DAO.Auth;
using ExamSystem.DAO.Token;
using ExamSystem.DTOs.Auth;
using ExamSystem.DTOs.Common;
using ExamSystem.Entity;
using ExamSystem.Exceptions;
using ExamSystem.Helpers;
using ExamSystem.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExamSystem.Services.Auth
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthDao _authDao;
        private readonly IAdminUserDao _adminUserDao;
        private readonly ITokenDao _tokenDao;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthManager(IAuthDao authDao, IAdminUserDao adminUserDao, ITokenDao tokenDao, IHttpContextAccessor httpContextAccessor)
        {
            _authDao = authDao;
            _adminUserDao = adminUserDao;
            _tokenDao = tokenDao;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response> LoginAsync(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
            {
                return Response.Error(new Error
                {
                    Status = 400,
                    Title = "Invalid Request",
                    Detail = "Username and password are required.",
                    InvalidParams = new List<InvalidParameter>
                    {
                        new InvalidParameter { Name = "username", Reason = "Username is required." },
                        new InvalidParameter { Name = "password", Reason = "Password is required." }
                    }
                });
            }

            var user = await _authDao.GetByUsernameAsync(request.username);
            if (user == null)
            {
                throw new UnauthorizedException("Invalid username or password.");
            }

            if (!user.is_active)
            {
                return Response.Error(new Error
                {
                    Status = 403,
                    Title = "Account Disabled",
                    Detail = "Your account has been disabled. Please contact admin."
                });
            }

            if (!Encryption.VerifyPassword(request.password, user.password_hash))
            {
                throw new UnauthorizedException("Invalid username or password.");
            }

            var sessionToken = TokensHelper.GenerateSessionToken();
            var jwtToken = TokensHelper.GenerateJwtToken(user.id, user.full_name ?? user.username, user.email, user.role, sessionToken);

            var tokenEntity = new m_token
            {
                user_id = user.id,
                session_token = sessionToken,
                jwt_token = jwtToken,
                expires_at = DateTime.UtcNow.AddMinutes(Consts.JwtDurationMinutes),
                created_at = DateTime.UtcNow,
                is_revoked = false,
                ip_address = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                user_agent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString()
            };
            await _tokenDao.Add(tokenEntity);

            var claims = TokensHelper.BuildCookieClaims(user.id, user.full_name ?? user.username, user.email, user.role, sessionToken);
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Response.Success(new AuthResponseDto
            {
                user_id = user.id,
                username = user.username,
                email = user.email,
                full_name = user.full_name,
                role = user.role,
                role_name = user.role.GetRoleName(),
                token = jwtToken,
                session_token = sessionToken
            });
        }

        public async Task<Response> RegisterAsync(RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
            {
                return Response.Error(new Error
                {
                    Status = 400,
                    Title = "Validation Failed",
                    Detail = "Username and password are required."
                });
            }

            if (await _authDao.UsernameExistsAsync(request.username))
            {
                return Response.Error(new Error
                {
                    Status = 409,
                    Title = "Duplicate Username",
                    Detail = "Username is already taken."
                });
            }

            if (!string.IsNullOrWhiteSpace(request.email) && await _authDao.EmailExistsAsync(request.email))
            {
                return Response.Error(new Error
                {
                    Status = 409,
                    Title = "Duplicate Email",
                    Detail = "Email is already registered."
                });
            }

            var user = new m_admin_user
            {
                username = request.username,
                email = request.email,
                password_hash = Encryption.HashPassword(request.password),
                full_name = request.full_name,
                phone = request.phone,
                role = request.role == 0 ? UserRoles.Admin : request.role,
                is_active = true,
                is_deleted = false,
                created_datetime = DateTime.Now,
                updated_datetime = DateTime.Now
            };

            await _authDao.RegisterAsync(user);
            return Response.Success(new { id = user.id, username = user.username });
        }

        public async Task<Response> LogoutAsync(long userId, string sessionToken)
        {
            var httpCtx = _httpContextAccessor.HttpContext;
            var tokenQuery = _tokenDao.GetAll();

            if (!string.IsNullOrWhiteSpace(sessionToken))
            {
                var specific = await tokenQuery
                    .Where(t => t.user_id == userId && t.session_token == sessionToken && !t.is_revoked)
                    .ToListAsync();
                foreach (var t in specific)
                {
                    t.is_revoked = true;
                    await _tokenDao.Update(t);
                }
            }

            if (httpCtx != null)
            {
                await httpCtx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return Response.Success("Logged out successfully.");
        }

        public async Task<Response> ForgotPasswordAsync(ForgotPasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.email))
            {
                return Response.Error(new Error { Status = 400, Title = "Invalid Request", Detail = "Email is required." });
            }

            var user = await _authDao.GetByEmailAsync(request.email);
            if (user != null)
            {
                var token = Encryption.GenerateRandomToken(48);
                user.password_reset_token = token;
                user.password_reset_expiry = DateTime.UtcNow.AddHours(24);
                user.updated_datetime = DateTime.Now;
                await _authDao.UpdateUserAsync(user);
                return Response.Success(new { reset_token = token, expires_in_hours = 24 });
            }

            return Response.Success("If the email exists, a reset link has been sent.");
        }

        public async Task<Response> UpdatePasswordByTokenAsync(UpdatePasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.email) || string.IsNullOrWhiteSpace(request.token)
                || string.IsNullOrWhiteSpace(request.new_password))
            {
                return Response.Error(new Error { Status = 400, Title = "Invalid Request", Detail = "All fields are required." });
            }

            var user = await _authDao.GetByResetTokenAsync(request.email, request.token);
            if (user == null)
            {
                return Response.Error(new Error { Status = 404, Title = "Invalid Token", Detail = "Invalid or expired token." });
            }

            user.password_hash = Encryption.HashPassword(request.new_password);
            user.password_reset_token = null;
            user.password_reset_expiry = null;
            user.updated_datetime = DateTime.Now;
            await _authDao.UpdateUserAsync(user);

            return Response.Success("Password updated successfully.");
        }

        public async Task<Response> ChangePasswordAsync(long userId, UpdatePasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.old_password) || string.IsNullOrWhiteSpace(request.new_password))
            {
                return Response.Error(new Error { Status = 400, Title = "Invalid Request", Detail = "Old and new passwords are required." });
            }

            if (request.new_password != request.confirm_password)
            {
                return Response.Error(new Error { Status = 400, Title = "Password Mismatch", Detail = "Confirm password does not match." });
            }

            var user = await _adminUserDao.GetById(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (!Encryption.VerifyPassword(request.old_password, user.password_hash))
            {
                return Response.Error(new Error { Status = 400, Title = "Invalid Password", Detail = "Old password is incorrect." });
            }

            user.password_hash = Encryption.HashPassword(request.new_password);
            user.updated_datetime = DateTime.Now;
            await _adminUserDao.Update(user);

            return Response.Success("Password changed successfully.");
        }
    }
}
