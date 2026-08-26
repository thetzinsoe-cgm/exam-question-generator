using ExamSystem.Provider;

namespace ExamSystem.Constraints;

public static class Auth
{
    private static IHttpContextAccessor _httpContextAccessor;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static bool IsAuthenticated =>
        _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public static string GetClaim(string claimType)
    {
        var user = _httpContextAccessor?.HttpContext?.User;
        return user?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}
