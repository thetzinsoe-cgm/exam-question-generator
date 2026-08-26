using ExamSystem.Constraints;
using ExamSystem.DTOs.Auth;
using ExamSystem.Exceptions;
using ExamSystem.Helpers;
using ExamSystem.Provider;
using ExamSystem.Services;
using ExamSystem.Services.Auth;
using ExamSystem.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamSystem.Controllers.Admin
{
    [Route("admin")]
    public class AuthController : BaseController
    {
        private readonly IAuthManager _authManager;
        private readonly SessionService _session;

        public AuthController(IAuthManager authManager, SessionService sessionService)
        {
            _authManager = authManager;
            _session = sessionService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View("~/Views/Auth/Login.cshtml", request);
            try
            {
                var resp = await _authManager.LoginAsync(request);
                if (resp.IsSuccess)
                {
                    SuccessMessage("Login successful.");
                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Dashboard");
                }
                if (resp.Errors != null)
                {
                    if (resp.Errors.InvalidParams != null && resp.Errors.InvalidParams.Any())
                    {
                        resp.Errors.InvalidParams.AddAuthLog();
                        ModelState.AddModelErrors(resp.Errors.InvalidParams);
                    }
                    else
                    {
                        ErrorMessage(resp.Errors.Detail);
                    }
                }
            }
            catch (UnauthorizedException uex)
            {
                ErrorMessage(uex.Message, "Login Failed");
            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message);
            }
            return View("~/Views/Auth/Login.cshtml", request);
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View("~/Views/Auth/Register.cshtml");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            if (!ModelState.IsValid) return View("~/Views/Auth/Register.cshtml", request);
            var resp = await _authManager.RegisterAsync(request);
            if (resp.IsSuccess)
            {
                SuccessMessage("Registration successful. Please login.");
                return RedirectToAction("Login");
            }
            if (resp.Errors != null)
            {
                if (resp.Errors.InvalidParams != null && resp.Errors.InvalidParams.Any())
                {
                    resp.Errors.InvalidParams.AddAuthLog();
                    ModelState.AddModelErrors(resp.Errors.InvalidParams);
                }
                else
                {
                    ErrorMessage(resp.Errors.Detail);
                }
            }
            return View("~/Views/Auth/Register.cshtml", request);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == UserClaims.Id)?.Value;
                var sessionToken = User.Claims.FirstOrDefault(c => c.Type == UserClaims.SessionToken)?.Value;
                if (long.TryParse(userIdClaim, out var uid))
                {
                    await _authManager.LogoutAsync(uid, sessionToken);
                }
                else
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
            catch { }
            return RedirectToAction("Login");
        }

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View("~/Views/Auth/ForgotPassword.cshtml");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
        {
            var resp = await _authManager.ForgotPasswordAsync(request);
            if (resp.IsSuccess)
            {
                var data = resp.Data;
                if (data != null)
                {
                    ViewBag.ResetToken = data.GetType().GetProperty("reset_token")?.GetValue(data)?.ToString();
                    ViewBag.ExpiresHours = data.GetType().GetProperty("expires_in_hours")?.GetValue(data)?.ToString();
                }
                SuccessMessage("If the email exists, a reset link is ready.");
            }
            else if (resp.Errors != null)
            {
                ErrorMessage(resp.Errors.Detail);
            }
            return View("~/Views/Auth/ForgotPassword.cshtml", request);
        }

        [HttpGet("update-password")]
        public IActionResult UpdatePassword([FromQuery] string token, [FromQuery] string email)
        {
            ViewBag.Token = token;
            ViewBag.Email = email;
            return View("~/Views/Auth/UpdatePassword.cshtml");
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDto request)
        {
            if (request.new_password != request.confirm_password)
            {
                ModelState.AddModelError("confirm_password", "Passwords do not match.");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Token = request.token;
                ViewBag.Email = request.email;
                return View("~/Views/Auth/UpdatePassword.cshtml", request);
            }
            var resp = await _authManager.UpdatePasswordByTokenAsync(request);
            if (resp.IsSuccess)
            {
                SuccessMessage("Password updated successfully.");
                return RedirectToAction("Login");
            }
            if (resp.Errors != null) ErrorMessage(resp.Errors.Detail);
            ViewBag.Token = request.token;
            ViewBag.Email = request.email;
            return View("~/Views/Auth/UpdatePassword.cshtml", request);
        }
    }
}
