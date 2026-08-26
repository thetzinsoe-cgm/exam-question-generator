using System;
using System.Collections.Generic;

namespace ExamSystem.Entity
{
    public class m_subject
    {
        public m_subject()
        {
            questions = new HashSet<m_question>();
            marking_rules = new HashSet<m_marking_rule>();
            exams = new HashSet<t_exam>();
        }

        public long id { get; set; }
        public long grade_id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public int total_marks { get; set; }
        public int pass_marks { get; set; }
        public int duration_minutes { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public long? created_user_id { get; set; }
        public long? updated_user_id { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }

        public virtual m_grade grade { get; set; }
        public virtual ICollection<m_question> questions { get; set; }
        public virtual ICollection<m_marking_rule> marking_rules { get; set; }
        public virtual ICollection<t_exam> exams { get; set; }
    }
}
