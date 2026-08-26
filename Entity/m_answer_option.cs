using System;

namespace ExamSystem.Entity
{
    public class m_answer_option
    {
        public long id { get; set; }
        public long question_id { get; set; }
        public string option_text { get; set; }
        public string option_html { get; set; }
        public string option_image_url { get; set; }
        public bool is_correct { get; set; }
        public decimal marks_allocated { get; set; }
        public int sort_order { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }

        public virtual m_question question { get; set; }
    }
}
