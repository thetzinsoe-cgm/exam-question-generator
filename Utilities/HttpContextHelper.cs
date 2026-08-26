namespace ExamSystem.Utilities
{
    public static class HttpContextHelper
    {
        private static IHttpContextAccessor _accessor;

        public static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public static IHttpContextAccessor CurrentHttpContextAccessor => _accessor;
        public static HttpContext Current => _accessor?.HttpContext;
    }
}
