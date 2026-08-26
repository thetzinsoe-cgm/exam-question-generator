using ExamSystem.Entity;

namespace ExamSystem.DAO.Token
{
    public interface ITokenDao
    {
        IQueryable<m_token> GetAll();
        Task<m_token> GetById(long id);
        Task Add(m_token token);
        Task Update(m_token token);
        Task<bool> IsTokenValidAsync(int userId, string sessionToken);
        Task RevokeAllTokensForUser(long userId);
    }
}
