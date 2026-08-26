using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ExamSystem.Exceptions;
using ExamSystem.Services.Token;

namespace ExamSystem.Attributes
{
    public class AuthorizeUserAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly short[] _roles;

        public AuthorizeUserAttribute(params short[] roles) : base("WebPolicy")
        {
            _roles = roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!await IsAuthorized(context))
            {
                throw new UnauthorizedException("Access denied: insufficient permissions.");
            }
        }

        private async Task<bool> IsAuthorized(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return false;
            }

            if (!await IsAuthorizedToken(context))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return false;
            }

            if (_roles == null || !_roles.Any())
            {
                return true;
            }

            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == "Role");
            if (roleClaim == null)
            {
                return false;
            }
            var userRole = Convert.ToInt16(roleClaim.Value);

            return _roles.Contains(userRole);
        }

        private async Task<bool> IsAuthorizedToken(AuthorizationFilterContext context)
        {
            var tokenService = context.HttpContext.RequestServices.GetService<ITokenService>();
            if (tokenService == null)
            {
                return true;
            }

            var user = context.HttpContext.User;
            var sessionToken = user.Claims.FirstOrDefault(c => c.Type == "SessionToken")?.Value?.ToString();
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "Identifer")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || string.IsNullOrWhiteSpace(sessionToken))
            {
                return false;
            }

            var userId = Convert.ToInt32(userIdClaim);
            return await tokenService.IsTokenValidAsync(userId, sessionToken);
        }
    }
}
