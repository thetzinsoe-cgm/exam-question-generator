using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.Subject
{
    public class SubjectDao : ISubjectDao
    {
        private readonly exam_system_entities _context;

        public SubjectDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public IQueryable<m_subject> GetAll()
        {
            return _context.m_subjects.Where(s => !s.is_deleted);
        }

        public Task<m_subject> GetById(long id)
        {
            return _context.m_subjects
                .Include(s => s.grade)
                .Include(s => s.questions)
                .Include(s => s.marking_rules)
                .Where(s => s.id == id && !s.is_deleted)
                .SingleOrDefaultAsync();
        }

        public async Task Add(m_subject subject)
        {
            _context.m_subjects.Add(subject);
            await _context.SaveChangesAsync();
        }

        public async Task Update(m_subject subject)
        {
            _context.m_subjects.Update(subject);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(m_subject subject)
        {
            subject.is_deleted = true;
            subject.updated_datetime = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public Task<bool> SubjectExists(long id)
        {
            return _context.m_subjects.AnyAsync(s => s.id == id && !s.is_deleted);
        }

        public Task<bool> CodeExists(string code, long? excludeId = null)
        {
            var query = _context.m_subjects
                .Where(s => !s.is_deleted && s.code.ToLower() == code.ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.id != excludeId.Value);
            }
            return query.AnyAsync();
        }
    }
}
