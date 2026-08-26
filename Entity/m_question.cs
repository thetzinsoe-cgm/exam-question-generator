using System;
using System.Collections.Generic;

namespace ExamSystem.Entity
{
    public class m_question
    {
        public m_question()
        {
            answer_options = new HashSet<m_answer_option>();
            exam_questions = new HashSet<t_exam_question>();
        }

        public long id { get; set; }
        public long subject_id { get; set; }
        public long grade_id { get; set; }
        public short question_type { get; set; }
        public string question_text { get; set; }
        public string question_html { get; set; }
        public string image_url { get; set; }
        public string hint { get; set; }
        public string explanation { get; set; }
        public short difficulty { get; set; }
        public decimal default_marks { get; set; }
        public decimal negative_marks { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public string eco_table_json { get; set; }
        public string tags_json { get; set; }
        public long? created_user_id { get; set; }
        public long? updated_user_id { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }

        public virtual m_subject subject { get; set; }
        public virtual m_grade grade { get; set; }
        public virtual ICollection<m_answer_option> answer_options { get; set; }
        public virtual ICollection<t_exam_question> exam_questions { get; set; }
    }
}
