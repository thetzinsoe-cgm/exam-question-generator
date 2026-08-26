using ExamSystem.DAO.Question;
using ExamSystem.DAO.Subject;
using ExamSystem.DAO.Grade;
using ExamSystem.DAO.Exam;
using ExamSystem.Entity;

namespace ExamSystem.DAO.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly exam_system_entities _context;

        public UnitOfWork(exam_system_entities context,
            IGradeDao gradeDao,
            ISubjectDao subjectDao,
            IQuestionDao questionDao,
            IExamDao examDao)
        {
            _context = context;
            Grades = gradeDao;
            Subjects = subjectDao;
            Questions = questionDao;
            Exams = examDao;
        }

        public exam_system_entities context => _context;
        public IGradeDao Grades { get; }
        public ISubjectDao Subjects { get; }
        public IQuestionDao Questions { get; }
        public IExamDao Exams { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
