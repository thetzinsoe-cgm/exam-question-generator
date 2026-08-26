namespace ExamSystem.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

namespace ExamSystem.Models.Auth
{
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class LoginResponse
    {
        public long user_id { get; set; }
        public string username { get; set; }
        public string token { get; set; }
    }

    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public short Role { get; set; }
    }
}
