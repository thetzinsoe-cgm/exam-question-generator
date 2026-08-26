using Microsoft.Extensions.Configuration;

namespace ExamSystem.Utilities
{
    public static class DatabaseHelper
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetProvider()
        {
            return _configuration["Database:Provider"];
        }

        public static string ConnectionString()
        {
            string provider = GetProvider();
            return provider switch
            {
                "MariaDb" => _configuration.GetConnectionString("MariaDb"),
                "SqlServer" => _configuration.GetConnectionString("SqlServer"),
                _ => throw new InvalidOperationException("Unsupported database provider.")
            };
        }
    }
}
