namespace ExamSystem.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("Unauthorized access.") { }
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, Exception inner) : base(message, inner) { }
    }

    public class UnauthorizedAdminException : UnauthorizedException
    {
        public UnauthorizedAdminException() : base("Admin access required.") { }
        public UnauthorizedAdminException(string message) : base(message) { }
    }
}
