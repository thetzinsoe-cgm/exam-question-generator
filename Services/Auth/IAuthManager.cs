using ExamSystem.DTOs.Auth;
using ExamSystem.DTOs.Common;

namespace ExamSystem.Services.Auth
{
    public interface IAuthManager
    {
        Task<Response> LoginAsync(LoginRequestDto request);
        Task<Response> RegisterAsync(RegisterRequestDto request);
        Task<Response> LogoutAsync(long userId, string sessionToken);
        Task<Response> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<Response> UpdatePasswordByTokenAsync(UpdatePasswordDto request);
        Task<Response> ChangePasswordAsync(long userId, UpdatePasswordDto request);
    }
}
