namespace ExamSystem.Utilities
{
    public class FilePathHelper
    {
        private readonly IWebHostEnvironment _env;

        public FilePathHelper(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string UploadRoot => Path.Combine(_env.WebRootPath, "uploads");

        public string QuestionImagesFolder => Ensure(Path.Combine(UploadRoot, "questions"));

        public string GetRelativePath(string absolutePath)
        {
            if (string.IsNullOrWhiteSpace(absolutePath)) return string.Empty;
            var relative = absolutePath.Replace(_env.WebRootPath, string.Empty)
                                        .Replace("\\", "/")
                                        .TrimStart('/');
            return "/" + relative;
        }

        private static string Ensure(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}
