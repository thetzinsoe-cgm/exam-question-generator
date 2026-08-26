using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.ExamQuestion
{
    public class ExamQuestionDao : IExamQuestionDao
    {
        private readonly exam_system_entities _context;

        public ExamQuestionDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public async Task AddRange(IEnumerable<t_exam_question> questions)
        {
            _context.t_exam_questions.AddRange(questions);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByExamId(long examId)
        {
            var records = await _context.t_exam_questions
                .Where(eq => eq.exam_id == examId && !eq.is_deleted)
                .ToListAsync();
            foreach (var eq in records)
            {
                eq.is_deleted = true;
            }
            _context.t_exam_questions.UpdateRange(records);
            await _context.SaveChangesAsync();
        }
    }
}
