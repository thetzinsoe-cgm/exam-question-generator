using ExamSystem.DTOs.Common;

namespace ExamSystem.DTOs.Exam
{
    public class ExamFilterDto : BaseFilterDto
    {
        public string search { get; set; }
        public long? subject_id { get; set; }
        public long? grade_id { get; set; }
        public bool? is_active { get; set; }
    }

    public class ExamGenerateRequestDto
    {
        private string _title;
        private string _description;

        public string title
        {
            get => _title;
            set => _title = value?.Trim();
        }

        public string description
        {
            get => _description;
            set => _description = value?.Trim();
        }

        public string exam_year { get; set; }
        public string examination_center { get; set; }
        public long subject_id { get; set; }
        public long grade_id { get; set; }
        public int total_questions { get; set; }
        public int duration_minutes { get; set; }
        public decimal pass_marks { get; set; }
        public List<ExamSectionDto> sections { get; set; } = new List<ExamSectionDto>();
        public bool use_marking_rules { get; set; } = true;
        public bool randomize_questions { get; set; } = true;
    }

    public class ExamSectionDto
    {
        public string section_name { get; set; }
        public short question_type { get; set; }
        public int question_count { get; set; }
        public short? difficulty { get; set; }
        public decimal marks_per_question { get; set; }
    }

    public class ExamResponseDto
    {
        public long id { get; set; }
        public string exam_code { get; set; }
        public string title { get; set; }
        public string exam_year { get; set; }
        public string examination_center { get; set; }
        public long subject_id { get; set; }
        public string subject_name { get; set; }
        public long grade_id { get; set; }
        public string grade_name { get; set; }
        public int total_questions { get; set; }
        public int duration_minutes { get; set; }
        public decimal total_marks { get; set; }
        public decimal pass_marks { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }
        public List<ExamQuestionDto> questions { get; set; } = new List<ExamQuestionDto>();
    }

    public class ExamQuestionDto
    {
        public long exam_question_id { get; set; }
        public long question_id { get; set; }
        public int question_number { get; set; }
        public string section_name { get; set; }
        public decimal marks_allocated { get; set; }
        public short question_type { get; set; }
        public string question_type_name { get; set; }
        public string question_text { get; set; }
        public string question_html { get; set; }
        public string image_url { get; set; }
        public string eco_table_json { get; set; }
        public List<DTOs.Question.AnswerOptionDto> answer_options { get; set; } = new List<DTOs.Question.AnswerOptionDto>();
    }

    public class ManualExamGenerateRequestDto
    {
        private string _title;
        private string _description;

        public string title
        {
            get => _title;
            set => _title = value?.Trim();
        }

        public string description
        {
            get => _description;
            set => _description = value?.Trim();
        }

        public string exam_year { get; set; }
        public string examination_center { get; set; }
        public long subject_id { get; set; }
        public long grade_id { get; set; }
        public int duration_minutes { get; set; }
        public decimal pass_marks { get; set; }
        public List<ManualExamSectionDto> sections { get; set; } = new List<ManualExamSectionDto>();
    }

    public class ManualExamSectionDto
    {
        public string section_name { get; set; }
        public List<ManualExamQuestionDto> questions { get; set; } = new List<ManualExamQuestionDto>();
    }

    public class ManualExamQuestionDto
    {
        public long question_id { get; set; }
        public int question_number { get; set; }
        public decimal marks_allocated { get; set; }
        public string section_name { get; set; }
    }

    public class QuestionSearchDto
    {
        public long id { get; set; }
        public string question_text { get; set; }
        public string question_html { get; set; }
        public short question_type { get; set; }
        public string question_type_name { get; set; }
        public short difficulty { get; set; }
        public string difficulty_name { get; set; }
        public decimal default_marks { get; set; }
        public string image_url { get; set; }
        public bool has_answer_options { get; set; }
    }

    public class QuestionSearchRequestDto
    {
        public long subject_id { get; set; }
        public long grade_id { get; set; }
        public string search { get; set; }
        public short? question_type { get; set; }
        public short? difficulty { get; set; }
        public int page_number { get; set; } = 1;
        public int page_size { get; set; } = 20;
    }

    public class QuestionSearchResponseDto
    {
        public List<QuestionSearchDto> questions { get; set; } = new List<QuestionSearchDto>();
        public int total { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public int total_pages { get; set; }
    }
}
