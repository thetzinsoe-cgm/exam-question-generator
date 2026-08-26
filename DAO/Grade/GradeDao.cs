using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.Grade
{
    public class GradeDao : IGradeDao
    {
        private readonly exam_system_entities _context;

        public GradeDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public IQueryable<m_grade> GetAll()
        {
            return _context.m_grades.Where(g => !g.is_deleted);
        }

        public Task<m_grade> GetById(long id)
        {
            return _context.m_grades
                .Include(g => g.subjects)
                .Where(g => g.id == id && !g.is_deleted)
                .SingleOrDefaultAsync();
        }

        public async Task Add(m_grade grade)
        {
            _context.m_grades.Add(grade);
            await _context.SaveChangesAsync();
        }

        public async Task Update(m_grade grade)
        {
            _context.m_grades.Update(grade);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(m_grade grade)
        {
            grade.is_deleted = true;
            grade.updated_datetime = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public Task<bool> GradeExists(long id)
        {
            return _context.m_grades.AnyAsync(g => g.id == id && !g.is_deleted);
        }

        public Task<bool> GradeNameExists(string name, long? excludeId = null)
        {
            var query = _context.m_grades
                .Where(g => !g.is_deleted && g.name.ToLower() == name.ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(g => g.id != excludeId.Value);
            }
            return query.AnyAsync();
        }
    }
}
