using ExamSystem.DAO.Dashboard;
using ExamSystem.DTOs.Common;
using ExamSystem.DTOs.Dashboard;

namespace ExamSystem.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<Response> GetMetricsAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardDao _dashboardDao;

        public DashboardService(IDashboardDao dashboardDao)
        {
            _dashboardDao = dashboardDao;
        }

        public async Task<Response> GetMetricsAsync()
        {
            var metrics = await _dashboardDao.GetMetricsAsync();
            return Response.Success(metrics);
        }
    }
}
