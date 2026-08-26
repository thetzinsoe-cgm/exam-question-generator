using System.Text.RegularExpressions;

namespace ExamSystem.Utilities
{
    public static class MathExpressionHelper
    {
        private static readonly Regex TexInlinePattern = new Regex(@"\$(.+?)\$", RegexOptions.Compiled);
        private static readonly Regex TexBlockPattern = new Regex(@"\$\$(.+?)\$\$", RegexOptions.Compiled | RegexOptions.Singleline);

        public static bool ContainsLatex(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return false;
            return TexInlinePattern.IsMatch(content) || TexBlockPattern.IsMatch(content);
        }

        public static string WrapWithKatexContainer(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return content;
            content = TexBlockPattern.Replace(content, m => $"<div class=\"katex-display\">$${m.Groups[1].Value}$$</div>");
            content = TexInlinePattern.Replace(content, m => $"<span class=\"katex-inline\">${m.Groups[1].Value}$</span>");
            return content;
        }
    }
}
