using System.Collections.Generic;
using ExamSystem.Entity;

namespace ExamSystem.DAO.ExamQuestion
{
    public interface IExamQuestionDao
    {
        Task AddRange(IEnumerable<t_exam_question> questions);
        Task DeleteByExamId(long examId);
        Task<List<t_exam_question>> GetByExamId(long examId);
        Task UpdateRange(IEnumerable<t_exam_question> questions);
    }
}
