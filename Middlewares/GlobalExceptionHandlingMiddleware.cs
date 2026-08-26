using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using ExamSystem.DTOs.Common;
using ExamSystem.Exceptions;

namespace ExamSystem.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            Response response;

            switch (exception)
            {
                case NotFoundException nfEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = Response.Error(new Error
                    {
                        Status = 404,
                        Title = "Not Found",
                        Detail = nfEx.Message
                    });
                    break;
                case UnauthorizedException uaEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = Response.Error(new Error
                    {
                        Status = 401,
                        Title = "Unauthorized",
                        Detail = uaEx.Message
                    });
                    if (!context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.Redirect("/admin/login");
                        return;
                    }
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = Response.Error(new Error
                    {
                        Status = 500,
                        Title = "Internal Server Error",
                        Detail = exception.Message
                    });
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
