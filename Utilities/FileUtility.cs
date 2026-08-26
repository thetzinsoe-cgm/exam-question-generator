using ExamSystem.Constraints;

namespace ExamSystem.Utilities
{
    public static class FileUtility
    {
        public static bool IsAllowedImageExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            var allowed = Consts.AllowedImageExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(s => s.Trim().ToLowerInvariant());
            return allowed.Contains(ext);
        }

        public static bool IsWithinSizeLimit(long bytes)
        {
            var max = Consts.MaxImageSizeMB * 1024 * 1024;
            return bytes <= max;
        }

        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "file";
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return $"{Guid.NewGuid():N}_{name}{ext}";
        }
    }
}
