using System;

namespace ExamSystem.Entity
{
    public class m_marking_rule
    {
        public long id { get; set; }
        public long subject_id { get; set; }
        public short question_type { get; set; }
        public decimal marks_per_question { get; set; }
        public decimal negative_marks { get; set; }
        public int min_questions { get; set; }
        public int max_questions { get; set; }
        public short difficulty { get; set; }
        public string rule_name { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public long? created_user_id { get; set; }
        public long? updated_user_id { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }

        public virtual m_subject subject { get; set; }
    }
}
