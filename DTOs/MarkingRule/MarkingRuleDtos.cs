using ExamSystem.DTOs.Common;

namespace ExamSystem.DTOs.MarkingRule
{
    public class MarkingRuleFilterDto : BaseFilterDto
    {
        public long? subject_id { get; set; }
        public short? question_type { get; set; }
        public bool? is_active { get; set; }
    }

    public class MarkingRuleRequestDto
    {
        private string _ruleName;
        private string _description;

        public string rule_name
        {
            get => _ruleName;
            set => _ruleName = value?.Trim();
        }

        public string description
        {
            get => _description;
            set => _description = value?.Trim();
        }

        public long subject_id { get; set; }
        public short question_type { get; set; }
        public decimal marks_per_question { get; set; }
        public decimal negative_marks { get; set; }
        public int min_questions { get; set; }
        public int max_questions { get; set; }
        public short difficulty { get; set; }
        public bool is_active { get; set; } = true;
    }

    public class MarkingRuleResponseDto
    {
        public long id { get; set; }
        public long subject_id { get; set; }
        public string subject_name { get; set; }
        public short question_type { get; set; }
        public string question_type_name { get; set; }
        public string rule_name { get; set; }
        public string description { get; set; }
        public decimal marks_per_question { get; set; }
        public decimal negative_marks { get; set; }
        public int min_questions { get; set; }
        public int max_questions { get; set; }
        public short difficulty { get; set; }
        public bool is_active { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }
    }
}
