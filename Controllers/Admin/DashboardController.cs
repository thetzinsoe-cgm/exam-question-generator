using ExamSystem.Attributes;
using ExamSystem.Constraints;
using ExamSystem.DTOs.Dashboard;
using ExamSystem.Services;
using ExamSystem.Services.Dashboard;
using ExamSystem.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers.Admin
{
    [Route("admin")]
    [AuthorizeUser]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        private readonly SessionService _session;

        public DashboardController(IDashboardService dashboardService, SessionService sessionService)
        {
            _dashboardService = dashboardService;
            _session = sessionService;
        }

        [HttpGet("dashboard")]
        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            var resp = await _dashboardService.GetMetricsAsync();
            var dto = resp.Data as DashboardMetricsDto ?? new DashboardMetricsDto();
            var model = new DashboardViewModel
            {
                TotalGrade = dto.TotalGrade,
                TotalSubject = dto.TotalSubject,
                TotalQuestionCount = dto.TotalQuestionCount,
                TotalExam = dto.TotalExam,
                TotalActiveAdmin = dto.TotalActiveAdmin,
                TotalMarkingRules = dto.TotalMarkingRules
            };
            return View("~/Views/Dashboard/Index.cshtml", model);
        }
    }
}
