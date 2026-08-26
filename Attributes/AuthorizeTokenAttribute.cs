using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ExamSystem.Exceptions;
using ExamSystem.Services.Token;

namespace ExamSystem.Attributes
{
    public class AuthorizeTokenAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public AuthorizeTokenAttribute() : base("ApiPolicy") { }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedException();
            }

            var tokenService = context.HttpContext.RequestServices.GetService<ITokenService>();
            if (tokenService == null)
            {
                return;
            }

            var sessionToken = user.Claims.FirstOrDefault(c => c.Type == "SessionToken")?.Value?.ToString();
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "Identifer")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || string.IsNullOrWhiteSpace(sessionToken))
            {
                throw new UnauthorizedException();
            }

            var userId = Convert.ToInt32(userIdClaim);
            if (!await tokenService.IsTokenValidAsync(userId, sessionToken))
            {
                throw new UnauthorizedException();
            }
        }
    }
}
