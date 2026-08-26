using System;

namespace ExamSystem.Entity
{
    public class m_token
    {
        public long id { get; set; }
        public long user_id { get; set; }
        public string session_token { get; set; }
        public string jwt_token { get; set; }
        public DateTime expires_at { get; set; }
        public DateTime created_at { get; set; }
        public bool is_revoked { get; set; }
        public string ip_address { get; set; }
        public string user_agent { get; set; }

        public virtual m_admin_user user { get; set; }
    }
}
