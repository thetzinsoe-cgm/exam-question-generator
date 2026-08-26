using ExamSystem.Attributes;
using ExamSystem.Constraints;
using ExamSystem.DTOs.Subject;
using ExamSystem.Exceptions;
using ExamSystem.Helpers;
using ExamSystem.Services;
using ExamSystem.Services.Grade;
using ExamSystem.Services.Subject;
using ExamSystem.Utilities;
using ExamSystem.ViewModels.Subject;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers.Admin
{
    [Route("admin")]
    [AuthorizeUser]
    public class SubjectController : BaseController
    {
        private readonly ISubjectService _service;
        private readonly IGradeService _gradeService;
        private readonly SessionService _session;

        public SubjectController(ISubjectService service, IGradeService gradeService, SessionService sessionService)
        {
            _service = service;
            _gradeService = gradeService;
            _session = sessionService;
        }

        private async Task LoadViewBags()
        {
            var grades = await _gradeService.GetAllForDropdownAsync();
            ViewBag.Grades = grades.Data;
        }

        [HttpGet("subject/index")]
        public async Task<IActionResult> Index([FromQuery] SubjectFilterDto filter)
        {
            filter ??= new SubjectFilterDto();
            var (list, total) = await _service.GetSubjectsAsync(filter);
            var paginated = PaginationHelper.Paginated(list, filter.page_number, filter.page_size, total, HttpContext.Request);
            await LoadViewBags();
            var vm = new SubjectIndexViewModel
            {
                Filter = filter,
                PageSizeOptions = DropDownHelper.GetPageSizeOptions(filter.page_size),
                Response = paginated
            };
            return View("~/Views/Subject/Index.cshtml", vm);
        }

        [HttpGet("subject/create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.FormTitle = "Create Subject";
            await LoadViewBags();
            return View("~/Views/Subject/Create.cshtml");
        }

        [HttpPost("subject/create")]
        public async Task<IActionResult> Create(SubjectRequestDto request)
        {
            ViewBag.FormTitle = "Create Subject";
            if (!ModelState.IsValid) { await LoadViewBags(); return View("~/Views/Subject/Create.cshtml", request); }
            var resp = await _service.CreateSubjectAsync(request);
            if (!resp.IsSuccess)
            {
                if (resp.Errors != null)
                {
                    if (resp.Errors.InvalidParams != null && resp.Errors.InvalidParams.Any())
                    {
                        resp.Errors.InvalidParams.AddAuthLog();
                        ModelState.AddModelErrors(resp.Errors.InvalidParams);
                    }
                    else ErrorMessage(resp.Errors.Detail);
                }
                await LoadViewBags();
                return View("~/Views/Subject/Create.cshtml", request);
            }
            SuccessMessage("Subject created successfully.");
            return RedirectToAction("Create");
        }

        [HttpGet("subject/edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            ViewBag.FormTitle = "Edit Subject";
            var resp = await _service.GetSubjectAsync(id);
            if (resp.Errors != null) return NotFound();
            await LoadViewBags();
            return View("~/Views/Subject/Edit.cshtml", resp.Data as SubjectResponseDto);
        }

        [HttpPost("subject/edit/{id}")]
        public async Task<IActionResult> Edit(long id, SubjectResponseDto input, string listingPageUrl)
        {
            ViewBag.FormTitle = "Edit Subject";
            var request = new SubjectRequestDto
            {
                grade_id = input.grade_id,
                name = input.name,
                code = input.code,
                description = input.description,
                total_marks = input.total_marks,
                pass_marks = input.pass_marks,
                duration_minutes = input.duration_minutes,
                is_active = input.is_active
            };
            var resp = await _service.UpdateSubjectAsync(id, request);
            if (!resp.IsSuccess)
            {
                if (resp.Errors != null)
                {
                    if (resp.Errors.InvalidParams != null && resp.Errors.InvalidParams.Any())
                    {
                        resp.Errors.InvalidParams.AddAuthLog();
                        ModelState.AddModelErrors(resp.Errors.InvalidParams);
                    }
                    else ErrorMessage(resp.Errors.Detail);
                }
                await LoadViewBags();
                return View("~/Views/Subject/Edit.cshtml", input);
            }
            SuccessMessage("Subject updated successfully.");
            return !string.IsNullOrWhiteSpace(listingPageUrl) ? Redirect(listingPageUrl) : RedirectToAction("Index");
        }

        [HttpGet("subject/delete/{id}")]
        public async Task<IActionResult> Delete(long id, int page_number = 1)
        {
            try
            {
                var resp = await _service.DeleteSubjectAsync(id);
                if (resp.IsSuccess) SuccessMessage("Subject deleted successfully.");
                else if (resp.Errors != null) ErrorMessage(resp.Errors.Detail);
            }
            catch (NotFoundException nfex) { ErrorMessage(nfex.Message); }
            return RedirectToAction("Index", new { page_number });
        }

        [HttpGet("subject/get-by-grade/{gradeId}")]
        public async Task<IActionResult> GetByGradeId(long gradeId)
        {
            var resp = await _service.GetByGradeIdAsync(gradeId);
            return Json(resp.Data);
        }
    }
}
