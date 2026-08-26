using ExamSystem.DTOs.Dashboard;

namespace ExamSystem.DAO.Dashboard
{
    public interface IDashboardDao
    {
        Task<DashboardMetricsDto> GetMetricsAsync();
    }
}
