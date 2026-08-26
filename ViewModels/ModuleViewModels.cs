using ExamSystem.DTOs;
using ExamSystem.DTOs.Grade;
using ExamSystem.DTOs.Subject;
using ExamSystem.DTOs.Question;
using ExamSystem.DTOs.MarkingRule;
using ExamSystem.DTOs.Exam;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExamSystem.ViewModels.Grade
{
    public class GradeIndexViewModel
    {
        public GradeFilterDto Filter { get; set; }
        public Paginated<GradeResponseDto> Response { get; set; }
        public List<SelectListItem> PageSizeOptions { get; set; }
    }
}

namespace ExamSystem.ViewModels.Subject
{
    public class SubjectIndexViewModel
    {
        public SubjectFilterDto Filter { get; set; }
        public Paginated<SubjectResponseDto> Response { get; set; }
        public List<SelectListItem> PageSizeOptions { get; set; }
    }
}

namespace ExamSystem.ViewModels.Question
{
    public class QuestionIndexViewModel
    {
        public QuestionFilterDto Filter { get; set; }
        public Paginated<QuestionResponseDto> Response { get; set; }
        public List<SelectListItem> PageSizeOptions { get; set; }
    }
}

namespace ExamSystem.ViewModels.MarkingRule
{
    public class MarkingRuleIndexViewModel
    {
        public MarkingRuleFilterDto Filter { get; set; }
        public Paginated<MarkingRuleResponseDto> Response { get; set; }
        public List<SelectListItem> PageSizeOptions { get; set; }
    }
}

namespace ExamSystem.ViewModels.Exam
{
    public class ExamIndexViewModel
    {
        public ExamFilterDto Filter { get; set; }
        public Paginated<ExamResponseDto> Response { get; set; }
        public List<SelectListItem> PageSizeOptions { get; set; }
    }
}
