using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.Auth
{
    public class AuthDao : IAuthDao
    {
        private readonly exam_system_entities _context;

        public AuthDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public Task<m_admin_user> GetByUsernameAsync(string username)
        {
            return _context.m_admin_users
                .Where(u => !u.is_deleted && u.username.ToLower() == username.ToLower())
                .SingleOrDefaultAsync();
        }

        public Task<m_admin_user> GetByEmailAsync(string email)
        {
            return _context.m_admin_users
                .Where(u => !u.is_deleted && u.email.ToLower() == email.ToLower())
                .SingleOrDefaultAsync();
        }

        public Task<m_admin_user> GetByResetTokenAsync(string email, string token)
        {
            return _context.m_admin_users
                .Where(u => !u.is_deleted
                            && u.email.ToLower() == email.ToLower()
                            && u.password_reset_token == token
                            && u.password_reset_expiry.HasValue
                            && u.password_reset_expiry.Value > DateTime.UtcNow)
                .SingleOrDefaultAsync();
        }

        public Task<bool> UsernameExistsAsync(string username)
        {
            return _context.m_admin_users
                .AnyAsync(u => !u.is_deleted && u.username.ToLower() == username.ToLower());
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return _context.m_admin_users
                .AnyAsync(u => !u.is_deleted && u.email.ToLower() == email.ToLower());
        }

        public async Task RegisterAsync(m_admin_user user)
        {
            _context.m_admin_users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(m_admin_user user)
        {
            _context.m_admin_users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
