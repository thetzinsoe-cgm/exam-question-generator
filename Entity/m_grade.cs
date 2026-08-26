using System;
using System.Collections.Generic;

namespace ExamSystem.Entity
{
    public class m_grade
    {
        public m_grade()
        {
            subjects = new HashSet<m_subject>();
        }

        public long id { get; set; }
        public string name { get; set; }
        public string level { get; set; }
        public string description { get; set; }
        public int sort_order { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public long? created_user_id { get; set; }
        public long? updated_user_id { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }

        public virtual ICollection<m_subject> subjects { get; set; }
    }
}
