using Microsoft.AspNetCore.Hosting;

namespace ExamSystem.Utilities
{
    public static class DocumentExportHelper
    {
        private static IWebHostEnvironment _environment;

        public static void Initialize(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public static string WebRootPath => _environment?.WebRootPath ?? Directory.GetCurrentDirectory();
        public static string ContentRootPath => _environment?.ContentRootPath ?? Directory.GetCurrentDirectory();

        public static string GetTemplatesFolder()
        {
            var path = Path.Combine(WebRootPath, "Report", "Templates");
            Directory.CreateDirectory(path);
            return path;
        }

        public static string GetTempExportFolder()
        {
            var path = Path.Combine(WebRootPath, "exports", "temp");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
