using System;

namespace ExamSystem.Entity
{
    public class t_exam_question
    {
        public long id { get; set; }
        public long exam_id { get; set; }
        public long question_id { get; set; }
        public int question_number { get; set; }
        public decimal marks_allocated { get; set; }
        public string section_name { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created_datetime { get; set; }

        public virtual t_exam exam { get; set; }
        public virtual m_question question { get; set; }
    }
}
