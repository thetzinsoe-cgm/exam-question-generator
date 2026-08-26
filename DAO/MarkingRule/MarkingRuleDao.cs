using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.MarkingRule
{
    public class MarkingRuleDao : IMarkingRuleDao
    {
        private readonly exam_system_entities _context;

        public MarkingRuleDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public IQueryable<m_marking_rule> GetAll()
        {
            return _context.m_marking_rules.Where(r => !r.is_deleted);
        }

        public Task<m_marking_rule> GetById(long id)
        {
            return _context.m_marking_rules
                .Include(r => r.subject)
                .Where(r => r.id == id && !r.is_deleted)
                .SingleOrDefaultAsync();
        }

        public async Task Add(m_marking_rule rule)
        {
            _context.m_marking_rules.Add(rule);
            await _context.SaveChangesAsync();
        }

        public async Task Update(m_marking_rule rule)
        {
            _context.m_marking_rules.Update(rule);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(m_marking_rule rule)
        {
            rule.is_deleted = true;
            rule.updated_datetime = DateTime.Now;
            _context.m_marking_rules.Update(rule);
            await _context.SaveChangesAsync();
        }

        public Task<bool> RuleExists(long id)
        {
            return _context.m_marking_rules.AnyAsync(r => r.id == id && !r.is_deleted);
        }

        public Task<List<m_marking_rule>> GetBySubjectId(long subjectId)
        {
            return _context.m_marking_rules
                .Where(r => !r.is_deleted && r.is_active && r.subject_id == subjectId)
                .ToListAsync();
        }
    }
}
