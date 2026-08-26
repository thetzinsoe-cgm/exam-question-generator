using ExamSystem.DTOs.MarkingRule;
using ExamSystem.Entity;

namespace ExamSystem.DAO.MarkingRule
{
    public interface IMarkingRuleDao
    {
        IQueryable<m_marking_rule> GetAll();
        Task<m_marking_rule> GetById(long id);
        Task Add(m_marking_rule rule);
        Task Update(m_marking_rule rule);
        Task Delete(m_marking_rule rule);
        Task<bool> RuleExists(long id);
        Task<List<m_marking_rule>> GetBySubjectId(long subjectId);
    }
}
