using ExamSystem.Entity;
using ExamSystem.DTOs.Auth;

namespace ExamSystem.DAO.Auth
{
    public interface IAuthDao
    {
        Task<m_admin_user> GetByUsernameAsync(string username);
        Task<m_admin_user> GetByEmailAsync(string email);
        Task<m_admin_user> GetByResetTokenAsync(string email, string token);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task RegisterAsync(m_admin_user user);
        Task UpdateUserAsync(m_admin_user user);
    }
}
