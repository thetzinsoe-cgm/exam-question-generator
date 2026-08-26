namespace ExamSystem.DTOs.Auth
{
    public class LoginRequestDto
    {
        private string _username;
        private string _password;

        public string username
        {
            get => _username;
            set => _username = value?.Trim();
        }

        public string password
        {
            get => _password;
            set => _password = value;
        }
    }

    public class RegisterRequestDto
    {
        private string _username;
        private string _email;
        private string _fullName;
        private string _password;

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

        public string password
        {
            get => _password;
            set => _password = value;
        }

        public short role { get; set; }
        public string phone { get; set; }
    }

    public class ForgotPasswordDto
    {
        private string _email;
        public string email
        {
            get => _email;
            set => _email = value?.Trim();
        }
    }

    public class UpdatePasswordDto
    {
        public string token { get; set; }
        public string email { get; set; }
        public string old_password { get; set; }
        public string new_password { get; set; }
        public string confirm_password { get; set; }
    }

    public class AuthResponseDto
    {
        public long user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string full_name { get; set; }
        public short role { get; set; }
        public string role_name { get; set; }
        public string token { get; set; }
        public string session_token { get; set; }
    }
}
