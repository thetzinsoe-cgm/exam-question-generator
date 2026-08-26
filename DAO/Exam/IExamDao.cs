using ExamSystem.DTOs.Exam;
using ExamSystem.Entity;

namespace ExamSystem.DAO.Exam
{
    public interface IExamDao
    {
        IQueryable<t_exam> GetAll();
        Task<t_exam> GetById(long id);
        Task<t_exam> GetByIdWithQuestions(long id);
        Task Add(t_exam exam);
        Task Update(t_exam exam);
        Task Delete(t_exam exam);
        Task<bool> ExamExists(long id);
        Task<bool> ExamCodeExists(string code, long? excludeId = null);
    }
}
