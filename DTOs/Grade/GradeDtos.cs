using ExamSystem.DTOs.Common;

namespace ExamSystem.DTOs.Grade
{
    public class GradeFilterDto : BaseFilterDto
    {
        public string search { get; set; }
        public bool? is_active { get; set; }
    }

    public class GradeRequestDto
    {
        private string _name;
        private string _level;
        private string _description;

        public string name
        {
            get => _name;
            set => _name = value?.Trim();
        }

        public string level
        {
            get => _level;
            set => _level = value?.Trim();
        }

        public string description
        {
            get => _description;
            set => _description = value?.Trim();
        }

        public int sort_order { get; set; }
        public bool is_active { get; set; } = true;
    }

    public class GradeResponseDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public string level { get; set; }
        public string description { get; set; }
        public int sort_order { get; set; }
        public bool is_active { get; set; }
        public int subject_count { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }
    }
}
