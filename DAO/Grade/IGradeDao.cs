using ExamSystem.DTOs.Grade;
using ExamSystem.Entity;

namespace ExamSystem.DAO.Grade
{
    public interface IGradeDao
    {
        IQueryable<m_grade> GetAll();
        Task<m_grade> GetById(long id);
        Task Add(m_grade grade);
        Task Update(m_grade grade);
        Task Delete(m_grade grade);
        Task<bool> GradeExists(long id);
        Task<bool> GradeNameExists(string name, long? excludeId = null);
    }
}
