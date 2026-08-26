using ExamSystem.Entity;

namespace ExamSystem.DAO.ExamQuestion
{
    public interface IExamQuestionDao
    {
        Task AddRange(IEnumerable<t_exam_question> questions);
        Task DeleteByExamId(long examId);
    }
}
