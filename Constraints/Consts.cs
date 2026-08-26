using Microsoft.Extensions.Configuration;

namespace ExamSystem.Constraints
{
    public static class Consts
    {
        private static IConfiguration _configuration;

        public static void Configure(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string JwtKey => _configuration["JwtSettings:Key"];
        public static string JwtIssuer => _configuration["JwtSettings:Issuer"];
        public static string JwtAudience => _configuration["JwtSettings:Audience"];
        public static int JwtDurationMinutes => int.TryParse(_configuration["JwtSettings:DurationMinutes"], out var d) ? d : 60;

        public static string UploadBaseUrl => _configuration["UploadSetting:BaseUrl"] ?? "/uploads/";

        public static int DefaultQuestionsPerExam => int.TryParse(_configuration["ExamSettings:DefaultQuestionsPerExam"], out var q) ? q : 50;
        public static int DefaultDurationMinutes => int.TryParse(_configuration["ExamSettings:DefaultDurationMinutes"], out var m) ? m : 120;
        public static string AllowedImageExtensions => _configuration["ExamSettings:AllowedImageExtensions"] ?? ".jpg,.jpeg,.png,.gif,.webp";
        public static int MaxImageSizeMB => int.TryParse(_configuration["ExamSettings:MaxImageSizeMB"], out var s) ? s : 5;
    }
}
