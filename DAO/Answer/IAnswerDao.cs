using ExamSystem.Entity;

namespace ExamSystem.DAO.Answer
{
    public interface IAnswerDao
    {
        Task<m_answer_option> GetById(long id);
        Task AddRange(IEnumerable<m_answer_option> options);
        Task UpdateRange(IEnumerable<m_answer_option> options);
        Task DeleteByQuestionId(long questionId);
    }
}
