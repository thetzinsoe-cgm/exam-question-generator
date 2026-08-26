using Microsoft.AspNetCore.Mvc.ModelBinding;
using ExamSystem.DTOs.Common;

namespace ExamSystem.Helpers
{
    public static class ErrorsHelper
    {
        public static void AddModelErrors(this ModelStateDictionary modelState, List<InvalidParameter> errors)
        {
            if (errors == null) return;
            foreach (var err in errors)
            {
                modelState.AddModelError(err.Name ?? string.Empty, err.Reason);
            }
        }

        public static List<InvalidParameter> GetErrors(this FluentValidation.Results.ValidationResult result)
        {
            return result.Errors.Select(e => new InvalidParameter
            {
                Name = e.PropertyName,
                Reason = e.ErrorMessage
            }).ToList();
        }

        public static void AddAuthLog(this List<InvalidParameter> errors)
        {
        }
    }
}
