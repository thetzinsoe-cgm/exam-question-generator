using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExamSystem.Utilities
{
    public class DropDownHelper
    {
        public static List<SelectListItem> GetPageSizeOptions(int selected = 10)
        {
            var sizes = new[] { 5, 10, 25, 50, 100 };
            return sizes.Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = $"{s} / page",
                Selected = s == selected
            }).ToList();
        }
    }
}
