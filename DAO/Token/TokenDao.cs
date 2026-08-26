using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.Token
{
    public class TokenDao : ITokenDao
    {
        private readonly exam_system_entities _context;

        public TokenDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public IQueryable<m_token> GetAll()
        {
            return _context.m_tokens.AsQueryable();
        }

        public Task<m_token> GetById(long id)
        {
            return _context.m_tokens.Where(x => x.id == id).SingleOrDefaultAsync();
        }

        public async Task Add(m_token token)
        {
            _context.m_tokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task Update(m_token token)
        {
            _context.m_tokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenValidAsync(int userId, string sessionToken)
        {
            return await _context.m_tokens
                .AnyAsync(t => t.user_id == userId
                            && t.session_token == sessionToken
                            && !t.is_revoked
                            && t.expires_at > DateTime.UtcNow);
        }

        public async Task RevokeAllTokensForUser(long userId)
        {
            var tokens = await _context.m_tokens
                .Where(t => t.user_id == userId && !t.is_revoked)
                .ToListAsync();
            foreach (var t in tokens)
            {
                t.is_revoked = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}
