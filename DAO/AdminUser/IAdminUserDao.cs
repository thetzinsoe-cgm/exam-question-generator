using ExamSystem.DTOs.AdminUser;
using ExamSystem.Entity;

namespace ExamSystem.DAO.AdminUser
{
    public interface IAdminUserDao
    {
        IQueryable<m_admin_user> GetAll();
        Task<m_admin_user> GetById(long id);
        Task Add(m_admin_user user);
        Task Update(m_admin_user user);
        Task Delete(m_admin_user user);
        Task<bool> UserExists(long id);
        Task<bool> UsernameExists(string username, long? excludeId = null);
        Task<bool> EmailExists(string email, long? excludeId = null);
    }
}
