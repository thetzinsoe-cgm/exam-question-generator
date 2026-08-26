using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ExamSystem.Constraints;
using ExamSystem.Provider;
using Microsoft.IdentityModel.Tokens;

namespace ExamSystem.Helpers
{
    public static class TokensHelper
    {
        public static string GenerateSessionToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string GenerateJwtToken(long userId, string name, string email, short role, string sessionToken, int? expiryMinutes = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Consts.JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(UserClaims.Id, userId.ToString()),
                new Claim(UserClaims.Name, name ?? string.Empty),
                new Claim(UserClaims.Email, email ?? string.Empty),
                new Claim(UserClaims.Role, role.ToString()),
                new Claim(UserClaims.SessionToken, sessionToken),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Consts.JwtIssuer,
                audience: Consts.JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes ?? Consts.JwtDurationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static List<Claim> BuildCookieClaims(long userId, string name, string email, short role, string sessionToken)
        {
            return new List<Claim>
            {
                new Claim(UserClaims.Id, userId.ToString()),
                new Claim(UserClaims.Name, name ?? string.Empty),
                new Claim(UserClaims.Email, email ?? string.Empty),
                new Claim(UserClaims.Role, role.ToString()),
                new Claim(UserClaims.SessionToken, sessionToken)
            };
        }
    }
}
