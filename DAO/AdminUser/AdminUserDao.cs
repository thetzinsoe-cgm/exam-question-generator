using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.AdminUser
{
    public class AdminUserDao : IAdminUserDao
    {
        private readonly exam_system_entities _context;

        public AdminUserDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public IQueryable<m_admin_user> GetAll()
        {
            return _context.m_admin_users.Where(u => !u.is_deleted);
        }

        public Task<m_admin_user> GetById(long id)
        {
            return _context.m_admin_users
                .Where(u => u.id == id && !u.is_deleted)
                .SingleOrDefaultAsync();
        }

        public async Task Add(m_admin_user user)
        {
            _context.m_admin_users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(m_admin_user user)
        {
            _context.m_admin_users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(m_admin_user user)
        {
            user.is_deleted = true;
            user.updated_datetime = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public Task<bool> UserExists(long id)
        {
            return _context.m_admin_users.AnyAsync(u => u.id == id && !u.is_deleted);
        }

        public Task<bool> UsernameExists(string username, long? excludeId = null)
        {
            var query = _context.m_admin_users
                .Where(u => !u.is_deleted && u.username.ToLower() == username.ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.id != excludeId.Value);
            }
            return query.AnyAsync();
        }

        public Task<bool> EmailExists(string email, long? excludeId = null)
        {
            var query = _context.m_admin_users
                .Where(u => !u.is_deleted && u.email.ToLower() == email.ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.id != excludeId.Value);
            }
            return query.AnyAsync();
        }
    }
}
