using System;
using System.Collections.Generic;

namespace ExamSystem.Entity
{
    public class t_exam
    {
        public t_exam()
        {
            exam_questions = new HashSet<t_exam_question>();
        }

        public long id { get; set; }
        public string exam_code { get; set; }
        public string title { get; set; }
        public string exam_year { get; set; }
        public string examination_center { get; set; }
        public long subject_id { get; set; }
        public long grade_id { get; set; }
        public int total_questions { get; set; }
        public int duration_minutes { get; set; }
        public decimal total_marks { get; set; }
        public decimal pass_marks { get; set; }
        public string description { get; set; }
        public string exam_config_json { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public long? created_user_id { get; set; }
        public long? updated_user_id { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }

        public virtual m_subject subject { get; set; }
        public virtual m_grade grade { get; set; }
        public virtual ICollection<t_exam_question> exam_questions { get; set; }
    }
}
