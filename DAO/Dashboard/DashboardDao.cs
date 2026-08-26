using ExamSystem.DTOs.Dashboard;
using ExamSystem.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.DAO.Dashboard
{
    public class DashboardDao : IDashboardDao
    {
        private readonly exam_system_entities _context;

        public DashboardDao(exam_system_entities dbContext)
        {
            _context = dbContext;
        }

        public async Task<DashboardMetricsDto> GetMetricsAsync()
        {
            return new DashboardMetricsDto
            {
                TotalGrade = await _context.m_grades.CountAsync(g => !g.is_deleted),
                TotalSubject = await _context.m_subjects.CountAsync(s => !s.is_deleted),
                TotalQuestionCount = await _context.m_questions.CountAsync(q => !q.is_deleted),
                TotalExam = await _context.t_exams.CountAsync(e => !e.is_deleted),
                TotalActiveAdmin = await _context.m_admin_users.CountAsync(u => !u.is_deleted && u.is_active),
                TotalMarkingRules = await _context.m_marking_rules.CountAsync(r => !r.is_deleted && r.is_active)
            };
        }
    }
}
