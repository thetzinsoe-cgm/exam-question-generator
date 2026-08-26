using ExamSystem.DTOs.Common;

namespace ExamSystem.DTOs.Subject
{
    public class SubjectFilterDto : BaseFilterDto
    {
        public string search { get; set; }
        public long? grade_id { get; set; }
        public bool? is_active { get; set; }
    }

    public class SubjectRequestDto
    {
        private string _name;
        private string _code;
        private string _description;

        public string name
        {
            get => _name;
            set => _name = value?.Trim();
        }

        public string code
        {
            get => _code;
            set => _code = value?.Trim();
        }

        public string description
        {
            get => _description;
            set => _description = value?.Trim();
        }

        public long grade_id { get; set; }
        public int total_marks { get; set; }
        public int pass_marks { get; set; }
        public int duration_minutes { get; set; }
        public bool is_active { get; set; } = true;
    }

    public class SubjectResponseDto
    {
        public long id { get; set; }
        public long grade_id { get; set; }
        public string grade_name { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public int total_marks { get; set; }
        public int pass_marks { get; set; }
        public int duration_minutes { get; set; }
        public bool is_active { get; set; }
        public int question_count { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }
    }
}
