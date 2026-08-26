using ExamSystem.DTOs.Common;

namespace ExamSystem.DTOs.AdminUser
{
    public class AdminUserFilterDto : BaseFilterDto
    {
        public string search { get; set; }
        public short? role { get; set; }
        public bool? is_active { get; set; }
    }

    public class AdminUserRequestDto
    {
        private string _username;
        private string _email;
        private string _fullName;
        private string _phone;

        public string username
        {
            get => _username;
            set => _username = value?.Trim();
        }

        public string email
        {
            get => _email;
            set => _email = value?.Trim();
        }

        public string full_name
        {
            get => _fullName;
            set => _fullName = value?.Trim();
        }

        public string phone
        {
            get => _phone;
            set => _phone = value?.Trim();
        }

        public string password { get; set; }
        public short role { get; set; }
        public bool is_active { get; set; } = true;
    }

    public class AdminUserResponseDto
    {
        public long id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public string phone { get; set; }
        public short role { get; set; }
        public string role_name { get; set; }
        public bool is_active { get; set; }
        public string profile_image { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_datetime { get; set; }
    }
}
