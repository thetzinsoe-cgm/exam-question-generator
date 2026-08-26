using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.Question
{
    public class QuestionDao : IQuestionDao
    {
        private readonly exam_system_entities _context;

        public QuestionDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public IQueryable<m_question> GetAll()
        {
            return _context.m_questions.Where(q => !q.is_deleted);
        }

        public Task<m_question> GetById(long id)
        {
            return _context.m_questions
                .Include(q => q.subject)
                .Include(q => q.grade)
                .Where(q => q.id == id && !q.is_deleted)
                .SingleOrDefaultAsync();
        }

        public Task<m_question> GetByIdWithAnswers(long id)
        {
            return _context.m_questions
                .Include(q => q.answer_options.Where(a => !a.is_deleted))
                .Include(q => q.subject)
                .Include(q => q.grade)
                .Where(q => q.id == id && !q.is_deleted)
                .SingleOrDefaultAsync();
        }

        public async Task Add(m_question question)
        {
            _context.m_questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task Update(m_question question)
        {
            _context.m_questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(m_question question)
        {
            question.is_deleted = true;
            question.updated_datetime = DateTime.Now;
            _context.m_questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public Task<bool> QuestionExists(long id)
        {
            return _context.m_questions.AnyAsync(q => q.id == id && !q.is_deleted);
        }

        public async Task<List<m_question>> GetRandomQuestionsBySubjectAndType(
            long subjectId,
            short questionType,
            short? difficulty,
            int count)
        {
            var query = _context.m_questions
                .Include(q => q.answer_options.Where(a => !a.is_deleted))
                .Where(q => !q.is_deleted
                            && q.is_active
                            && q.subject_id == subjectId
                            && q.question_type == questionType);

            if (difficulty.HasValue)
            {
                query = query.Where(q => q.difficulty == difficulty.Value);
            }

            return await query
                .OrderBy(q => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
        }
    }
}
