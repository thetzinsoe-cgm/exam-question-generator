using ExamSystem.DAO.Token;

namespace ExamSystem.Services.Token
{
    public interface ITokenService
    {
        Task<bool> IsTokenValidAsync(int userId, string sessionToken);
        Task RevokeAllTokensForUser(long userId);
    }

    public class TokenService : ITokenService
    {
        private readonly ITokenDao _tokenDao;

        public TokenService(ITokenDao tokenDao)
        {
            _tokenDao = tokenDao;
        }

        public async Task<bool> IsTokenValidAsync(int userId, string sessionToken)
        {
            return await _tokenDao.IsTokenValidAsync(userId, sessionToken);
        }

        public async Task RevokeAllTokensForUser(long userId)
        {
            await _tokenDao.RevokeAllTokensForUser(userId);
        }
    }
}
