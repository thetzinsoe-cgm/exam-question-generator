using Microsoft.AspNetCore.Http;

namespace ExamSystem.Middlewares
{
    public class PreventBackHistoryMiddleware
    {
        private readonly RequestDelegate _next;

        public PreventBackHistoryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "-1";
            await _next(context);
        }
    }
}
