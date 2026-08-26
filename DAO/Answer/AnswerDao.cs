using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.Answer
{
    public class AnswerDao : IAnswerDao
    {
        private readonly exam_system_entities _context;

        public AnswerDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public Task<m_answer_option> GetById(long id)
        {
            return _context.m_answer_options
                .Where(a => a.id == id && !a.is_deleted)
                .SingleOrDefaultAsync();
        }

        public async Task AddRange(IEnumerable<m_answer_option> options)
        {
            _context.m_answer_options.AddRange(options);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRange(IEnumerable<m_answer_option> options)
        {
            _context.m_answer_options.UpdateRange(options);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByQuestionId(long questionId)
        {
            var options = await _context.m_answer_options
                .Where(a => a.question_id == questionId && !a.is_deleted)
                .ToListAsync();
            foreach (var opt in options)
            {
                opt.is_deleted = true;
                opt.updated_datetime = DateTime.Now;
            }
            _context.m_answer_options.UpdateRange(options);
            await _context.SaveChangesAsync();
        }
    }
}
