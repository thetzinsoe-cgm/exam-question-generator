using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.Exam
{
    public class ExamDao : IExamDao
    {
        private readonly exam_system_entities _context;

        public ExamDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public IQueryable<t_exam> GetAll()
        {
            return _context.t_exams.Where(e => !e.is_deleted);
        }

        public Task<t_exam> GetById(long id)
        {
            return _context.t_exams
                .Include(e => e.subject)
                .Include(e => e.grade)
                .Where(e => e.id == id && !e.is_deleted)
                .SingleOrDefaultAsync();
        }

        public Task<t_exam> GetByIdWithQuestions(long id)
        {
            return _context.t_exams
                .Include(e => e.subject)
                .Include(e => e.grade)
                .Include(e => e.exam_questions.Where(eq => !eq.is_deleted))
                    .ThenInclude(eq => eq.question)
                        .ThenInclude(q => q.answer_options.Where(a => !a.is_deleted))
                .Where(e => e.id == id && !e.is_deleted)
                .SingleOrDefaultAsync();
        }

        public async Task Add(t_exam exam)
        {
            _context.t_exams.Add(exam);
            await _context.SaveChangesAsync();
        }

        public async Task Update(t_exam exam)
        {
            _context.t_exams.Update(exam);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(t_exam exam)
        {
            exam.is_deleted = true;
            exam.updated_datetime = DateTime.Now;
            _context.t_exams.Update(exam);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExamExists(long id)
        {
            return _context.t_exams.AnyAsync(e => e.id == id && !e.is_deleted);
        }

        public Task<bool> ExamCodeExists(string code, long? excludeId = null)
        {
            var query = _context.t_exams
                .Where(e => !e.is_deleted && e.exam_code.ToLower() == code.ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(e => e.id != excludeId.Value);
            }
            return query.AnyAsync();
        }
    }
}
