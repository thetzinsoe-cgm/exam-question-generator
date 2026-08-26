using ExamSystem.DTOs.Subject;
using ExamSystem.Entity;

namespace ExamSystem.DAO.Subject
{
    public interface ISubjectDao
    {
        IQueryable<m_subject> GetAll();
        Task<m_subject> GetById(long id);
        Task Add(m_subject subject);
        Task Update(m_subject subject);
        Task Delete(m_subject subject);
        Task<bool> SubjectExists(long id);
        Task<bool> CodeExists(string code, long? excludeId = null);
    }
}
