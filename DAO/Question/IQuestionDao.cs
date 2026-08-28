using ExamSystem.DTOs.Question;
using ExamSystem.Entity;

namespace ExamSystem.DAO.Question
{
    public interface IQuestionDao
    {
        IQueryable<m_question> GetAll();
        Task<m_question> GetById(long id);
        Task<m_question> GetByIdWithAnswers(long id);
        Task Add(m_question question);
        Task Update(m_question question);
        Task Delete(m_question question);
        Task<bool> QuestionExists(long id);
        Task<List<m_question>> GetRandomQuestionsBySubjectAndType(long subjectId, short questionType, short? difficulty, int count);
        Task<List<m_question>> GetByIdsAsync(List<long> ids);
    }
}
