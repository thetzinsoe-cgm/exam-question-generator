using ExamSystem.Attributes;
using ExamSystem.Constraints;
using ExamSystem.DTOs.MarkingRule;
using ExamSystem.Exceptions;
using ExamSystem.Helpers;
using ExamSystem.Services;
using ExamSystem.Services.MarkingRule;
using ExamSystem.Services.Subject;
using ExamSystem.Utilities;
using ExamSystem.ViewModels.MarkingRule;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers.Admin
{
    [Route("admin")]
    [AuthorizeUser(UserRoles.SuperAdmin, UserRoles.Admin, UserRoles.Teacher)]
    public class MarkingRuleController : BaseController
    {
        private readonly IMarkingRuleService _service;
        private readonly ISubjectService _subjectService;
        private readonly SessionService _session;

        public MarkingRuleController(IMarkingRuleService service, ISubjectService subjectService, SessionService sessionService)
        {
            _service = service;
            _subjectService = subjectService;
            _session = sessionService;
        }

        private async Task LoadViewBags()
        {
            var subjects = await _subjectService.GetAllForDropdownFallback();
            ViewBag.Subjects = subjects;
            ViewBag.QuestionTypes = QuestionTypes.AllTypes()
                .Select(t => new { id = t, name = t.GetTypeName() })
                .ToList();
        }

        [HttpGet("marking-rule/index")]
        public async Task<IActionResult> Index([FromQuery] MarkingRuleFilterDto filter)
        {
            filter ??= new MarkingRuleFilterDto();
            var (list, total) = await _service.GetRulesAsync(filter);
            var paginated = PaginationHelper.Paginated(list, filter.page_number, filter.page_size, total, HttpContext.Request);
            await LoadViewBags();
            var vm = new MarkingRuleIndexViewModel
            {
                Filter = filter,
                PageSizeOptions = DropDownHelper.GetPageSizeOptions(filter.page_size),
                Response = paginated
            };
            return View("~/Views/MarkingRule/Index.cshtml", vm);
        }

        [HttpGet("marking-rule/create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.FormTitle = "Create Marking Rule";
            await LoadViewBags();
            return View("~/Views/MarkingRule/Create.cshtml");
        }

        [HttpPost("marking-rule/create")]
        public async Task<IActionResult> Create(MarkingRuleRequestDto request)
        {
            ViewBag.FormTitle = "Create Marking Rule";
            if (!ModelState.IsValid) { await LoadViewBags(); return View("~/Views/MarkingRule/Create.cshtml", request); }
            var resp = await _service.CreateRuleAsync(request);
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
                return View("~/Views/MarkingRule/Create.cshtml", request);
            }
            SuccessMessage("Marking rule created successfully.");
            return RedirectToAction("Create");
        }

        [HttpGet("marking-rule/edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            ViewBag.FormTitle = "Edit Marking Rule";
            var resp = await _service.GetRuleAsync(id);
            if (resp.Errors != null) return NotFound();
            await LoadViewBags();
            return View("~/Views/MarkingRule/Edit.cshtml", resp.Data as MarkingRuleResponseDto);
        }

        [HttpPost("marking-rule/edit/{id}")]
        public async Task<IActionResult> Edit(long id, MarkingRuleResponseDto input, string listingPageUrl)
        {
            ViewBag.FormTitle = "Edit Marking Rule";
            var request = new MarkingRuleRequestDto
            {
                subject_id = input.subject_id,
                question_type = input.question_type,
                marks_per_question = input.marks_per_question,
                negative_marks = input.negative_marks,
                min_questions = input.min_questions,
                max_questions = input.max_questions,
                difficulty = input.difficulty,
                rule_name = input.rule_name,
                description = input.description,
                is_active = input.is_active
            };
            var resp = await _service.UpdateRuleAsync(id, request);
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
                return View("~/Views/MarkingRule/Edit.cshtml", input);
            }
            SuccessMessage("Marking rule updated successfully.");
            return !string.IsNullOrWhiteSpace(listingPageUrl) ? Redirect(listingPageUrl) : RedirectToAction("Index");
        }

        [HttpGet("marking-rule/delete/{id}")]
        public async Task<IActionResult> Delete(long id, int page_number = 1)
        {
            try
            {
                var resp = await _service.DeleteRuleAsync(id);
                if (resp.IsSuccess) SuccessMessage("Marking rule deleted successfully.");
                else if (resp.Errors != null) ErrorMessage(resp.Errors.Detail);
            }
            catch (NotFoundException nfex) { ErrorMessage(nfex.Message); }
            return RedirectToAction("Index", new { page_number });
        }
    }

    public static class SubjectServiceExtensions
    {
        public static async Task<object> GetAllForDropdownFallback(this ISubjectService svc)
        {
            var list = new List<object>();
            try
            {
                var resp = await svc.GetByGradeIdAsync(0);
            }
            catch { }
            var response = await svc.GetByGradeIdAsync(0);
            return response.Data;
        }
    }
}
