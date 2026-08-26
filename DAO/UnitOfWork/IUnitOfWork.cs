using ExamSystem.DAO.Question;
using ExamSystem.DAO.Subject;
using ExamSystem.DAO.Grade;
using ExamSystem.DAO.Exam;
using ExamSystem.Entity;

namespace ExamSystem.DAO.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        exam_system_entities context { get; }
        IGradeDao Grades { get; }
        ISubjectDao Subjects { get; }
        IQuestionDao Questions { get; }
        IExamDao Exams { get; }
        Task<int> SaveChangesAsync();
    }
}
