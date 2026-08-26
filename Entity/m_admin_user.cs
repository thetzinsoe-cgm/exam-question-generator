using System;
using System.Collections.Generic;

namespace ExamSystem.Entity
{
    public class m_admin_user
    {
        public m_admin_user()
        {
            created_tokens = new HashSet<m_token>();
            created_questions = new HashSet<m_question>();
            created_exams = new HashSet<t_exam>();
        }

        public long id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password_hash { get; set; }
        public string full_name { get; set; }
        public string phone { get; set; }
        public string profile_image { get; set; }
        public short role { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public string password_reset_token { get; set; }
        public DateTime? password_reset_expiry { get; set; }
        public long? created_user_id { get; set; }
        public long? updated_user_id { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }

        public virtual ICollection<m_token> created_tokens { get; set; }
        public virtual ICollection<m_question> created_questions { get; set; }
        public virtual ICollection<t_exam> created_exams { get; set; }
    }
}
