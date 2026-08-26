using ExamSystem.Attributes;
using ExamSystem.Constraints;
using ExamSystem.DTOs.Grade;
using ExamSystem.Exceptions;
using ExamSystem.Helpers;
using ExamSystem.Services;
using ExamSystem.Services.Grade;
using ExamSystem.Utilities;
using ExamSystem.ViewModels.Grade;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers.Admin
{
    [Route("admin")]
    [AuthorizeUser]
    public class GradeController : BaseController
    {
        private readonly IGradeService _service;
        private readonly SessionService _session;

        public GradeController(IGradeService service, SessionService sessionService)
        {
            _service = service;
            _session = sessionService;
        }

        [HttpGet("grade/index")]
        public async Task<IActionResult> Index([FromQuery] GradeFilterDto filter)
        {
            filter ??= new GradeFilterDto();
            var (list, total) = await _service.GetGradesAsync(filter);
            var paginated = PaginationHelper.Paginated(list, filter.page_number, filter.page_size, total, HttpContext.Request);

            var vm = new GradeIndexViewModel
            {
                Filter = filter,
                PageSizeOptions = DropDownHelper.GetPageSizeOptions(filter.page_size),
                Response = paginated
            };
            return View("~/Views/Grade/Index.cshtml", vm);
        }

        [HttpGet("grade/create")]
        public IActionResult Create()
        {
            ViewBag.FormTitle = "Create Grade";
            return View("~/Views/Grade/Create.cshtml");
        }

        [HttpPost("grade/create")]
        public async Task<IActionResult> Create(GradeRequestDto request)
        {
            ViewBag.FormTitle = "Create Grade";
            if (!ModelState.IsValid) return View("~/Views/Grade/Create.cshtml", request);
            var resp = await _service.CreateGradeAsync(request);
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
                return View("~/Views/Grade/Create.cshtml", request);
            }
            SuccessMessage("Grade created successfully.");
            return RedirectToAction("Create");
        }

        [HttpGet("grade/edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            ViewBag.FormTitle = "Edit Grade";
            var resp = await _service.GetGradeAsync(id);
            if (resp.Errors != null) return NotFound();
            return View("~/Views/Grade/Edit.cshtml", resp.Data as GradeResponseDto);
        }

        [HttpPost("grade/edit/{id}")]
        public async Task<IActionResult> Edit(long id, GradeResponseDto input, string listingPageUrl)
        {
            ViewBag.FormTitle = "Edit Grade";
            var request = new GradeRequestDto
            {
                name = input.name,
                level = input.level,
                description = input.description,
                sort_order = input.sort_order,
                is_active = input.is_active
            };
            var resp = await _service.UpdateGradeAsync(id, request);
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
                return View("~/Views/Grade/Edit.cshtml", input);
            }
            SuccessMessage("Grade updated successfully.");
            return !string.IsNullOrWhiteSpace(listingPageUrl) ? Redirect(listingPageUrl) : RedirectToAction("Index");
        }

        [HttpGet("grade/delete/{id}")]
        public async Task<IActionResult> Delete(long id, int page_number = 1)
        {
            try
            {
                var resp = await _service.DeleteGradeAsync(id);
                if (resp.IsSuccess) SuccessMessage("Grade deleted successfully.");
                else if (resp.Errors != null) ErrorMessage(resp.Errors.Detail);
            }
            catch (NotFoundException nfex) { ErrorMessage(nfex.Message); }
            return RedirectToAction("Index", new { page_number });
        }

        [HttpGet("grade/get-by-grade/{gradeId}")]
        public async Task<IActionResult> GetByGradeId(long gradeId)
        {
            var resp = await _service.GetAllForDropdownAsync();
            return Json(resp.Data);
        }
    }
}
