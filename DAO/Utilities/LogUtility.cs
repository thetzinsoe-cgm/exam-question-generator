using log4net;

namespace ExamSystem.DAO.Utilities
{
    public class LogUtility
    {
        private readonly ILog _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private LogUtility(ILog logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public static LogUtility CreateLogUtility(IHttpContextAccessor httpContextAccessor)
        {
            var logger = LogManager.GetLogger("MvcLogger");
            return new LogUtility(logger, httpContextAccessor);
        }

        public void Info(string url, string method, string message)
        {
            _logger.Info($"[{method}] {url} - {message}");
        }

        public void Warning(string url, string method, string message)
        {
            _logger.Warn($"[{method}] {url} - {message}");
        }

        public void LogException(Exception ex)
        {
            var ctx = _httpContextAccessor?.HttpContext;
            var path = ctx?.Request.Path.ToString() ?? "N/A";
            var method = ctx?.Request.Method ?? "N/A";
            _logger.Error($"[{method}] {path} - Exception: {ex.Message}", ex);
        }
    }
}
