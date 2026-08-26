using Microsoft.AspNetCore.Mvc.Filters;

namespace ExamSystem.Attributes
{
    public class TrimmedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var parameters = context.ActionArguments;
            foreach (var kvp in parameters.ToList())
            {
                if (kvp.Value is string strValue && strValue != null)
                {
                    context.ActionArguments[kvp.Key] = strValue.Trim();
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
