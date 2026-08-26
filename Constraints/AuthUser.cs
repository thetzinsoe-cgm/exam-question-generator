using ExamSystem.Exceptions;
using ExamSystem.Provider;

namespace ExamSystem.Constraints;

public static class AuthUser
{
    private static IHttpContextAccessor _httpContextAccessor;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static long Id => Convert.ToInt64(GetValue(UserClaims.Id) ?? "0");
    public static string Name => GetValue(UserClaims.Name)?.ToString();
    public static string Email => GetValue(UserClaims.Email)?.ToString();
    public static short Role => Convert.ToInt16(GetValue(UserClaims.Role) ?? "0");
    public static string SessionToken => GetValue(UserClaims.SessionToken)?.ToString();

    public static bool IsSuperAdmin => Role == UserRoles.SuperAdmin;
    public static bool IsAdmin => Role == UserRoles.Admin;
    public static bool IsTeacher => Role == UserRoles.Teacher;
    public static bool IsExaminer => Role == UserRoles.Examiner;

    private static object GetValue(string claimName)
    {
        var user = _httpContextAccessor?.HttpContext?.User;
        if (user is null || !user.Identity.IsAuthenticated)
        {
            throw new UnauthorizedException();
        }
        var valueClaim = user.Claims.FirstOrDefault(c => c.Type == claimName);
        if (valueClaim is null)
        {
            throw new NotFoundException($"Claim with Name:{claimName} was not found.");
        }
        return string.IsNullOrWhiteSpace(valueClaim.Value) ? null : valueClaim.Value;
    }
}
