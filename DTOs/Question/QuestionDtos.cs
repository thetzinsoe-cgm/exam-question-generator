using ExamSystem.DTOs.Common;

namespace ExamSystem.DTOs.Question
{
    public class QuestionFilterDto : BaseFilterDto
    {
        public string search { get; set; }
        public long? subject_id { get; set; }
        public long? grade_id { get; set; }
        public short? question_type { get; set; }
        public short? difficulty { get; set; }
        public bool? is_active { get; set; }
    }

    public class QuestionRequestDto
    {
        private string _questionText;
        private string _hint;
        private string _explanation;

        public string question_text
        {
            get => _questionText;
            set => _questionText = value?.Trim();
        }

        public string question_html { get; set; }

        public string hint
        {
            get => _hint;
            set => _hint = value?.Trim();
        }

        public string explanation
        {
            get => _explanation;
            set => _explanation = value?.Trim();
        }

        public long subject_id { get; set; }
        public long grade_id { get; set; }
        public short question_type { get; set; }
        public short difficulty { get; set; } = 2;
        public decimal default_marks { get; set; } = 1;
        public decimal negative_marks { get; set; } = 0;
        public bool is_active { get; set; } = true;
        public string eco_table_json { get; set; }
        public string tags_json { get; set; }
        public string image_url { get; set; }
        public List<AnswerOptionDto> answer_options { get; set; } = new List<AnswerOptionDto>();
    }

    public class AnswerOptionDto
    {
        public long id { get; set; }
        private string _optionText;

        public string option_text
        {
            get => _optionText;
            set => _optionText = value?.Trim();
        }

        public string option_html { get; set; }
        public string option_image_url { get; set; }
        public bool is_correct { get; set; }
        public decimal marks_allocated { get; set; }
        public int sort_order { get; set; }
        public bool is_deleted { get; set; }
    }

    public class QuestionResponseDto
    {
        public long id { get; set; }
        public long subject_id { get; set; }
        public string subject_name { get; set; }
        public long grade_id { get; set; }
        public string grade_name { get; set; }
        public short question_type { get; set; }
        public string question_type_name { get; set; }
        public string question_text { get; set; }
        public string question_html { get; set; }
        public string image_url { get; set; }
        public string hint { get; set; }
        public string explanation { get; set; }
        public short difficulty { get; set; }
        public decimal default_marks { get; set; }
        public decimal negative_marks { get; set; }
        public bool is_active { get; set; }
        public string eco_table_json { get; set; }
        public string tags_json { get; set; }
        public int sort_order { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }
        public List<AnswerOptionDto> answer_options { get; set; } = new List<AnswerOptionDto>();
    }

    public class BulkImportResultDto
    {
        public int TotalRows { get; set; }
        public int ImportedCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
