using ExamSystem.Attributes;
using ExamSystem.Constraints;
using ExamSystem.DTOs.Exam;
using ExamSystem.DTOs.Question;
using ExamSystem.Exceptions;
using ExamSystem.Helpers;
using ExamSystem.Services;
using ExamSystem.Services.Grade;
using ExamSystem.Services.Question;
using ExamSystem.Services.Subject;
using ExamSystem.Utilities;
using ExamSystem.ViewModels.Question;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers.Admin
{
    [Route("admin")]
    [AuthorizeUser]
    public class QuestionController : BaseController
    {
        private readonly IQuestionService _service;
        private readonly ISubjectService _subjectService;
        private readonly IGradeService _gradeService;
        private readonly SessionService _session;
        private readonly FilePathHelper _filePathHelper;

        public QuestionController(IQuestionService service, ISubjectService subjectService,
            IGradeService gradeService, SessionService sessionService, FilePathHelper filePathHelper)
        {
            _service = service;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _session = sessionService;
            _filePathHelper = filePathHelper;
        }

        private async Task LoadViewBags()
        {
            ViewBag.Grades = (await _gradeService.GetAllForDropdownAsync()).Data;
            ViewBag.QuestionTypes = QuestionTypes.AllTypes()
                .Select(t => new { id = t, name = t.GetTypeName() })
                .ToList();
        }

        [HttpGet("question/index")]
        public async Task<IActionResult> Index([FromQuery] QuestionFilterDto filter)
        {
            filter ??= new QuestionFilterDto();
            var (list, total) = await _service.GetQuestionsAsync(filter);
            var paginated = PaginationHelper.Paginated(list, filter.page_number, filter.page_size, total, HttpContext.Request);
            await LoadViewBags();
            ViewBag.Subjects = filter.grade_id.HasValue
                ? (await _subjectService.GetByGradeIdAsync(filter.grade_id.Value)).Data
                : null;

            var vm = new QuestionIndexViewModel
            {
                Filter = filter,
                PageSizeOptions = DropDownHelper.GetPageSizeOptions(filter.page_size),
                Response = paginated
            };
            return View("~/Views/Question/Index.cshtml", vm);
        }

        [HttpGet("question/create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.FormTitle = "Create Question";
            await LoadViewBags();
            return View("~/Views/Question/Create.cshtml");
        }

        [HttpPost("question/create")]
        [RequestFormLimits(ValueCountLimit = 2048)]
        public async Task<IActionResult> Create(QuestionRequestDto request)
        {
            ViewBag.FormTitle = "Create Question";
            if (!ModelState.IsValid) { await LoadViewBags(); return View("~/Views/Question/Create.cshtml", request); }
            var resp = await _service.CreateQuestionAsync(request);
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
                return View("~/Views/Question/Create.cshtml", request);
            }
            SuccessMessage("Question created successfully.");
            return RedirectToAction("Create");
        }

        [HttpGet("question/edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            ViewBag.FormTitle = "Edit Question";
            var resp = await _service.GetQuestionAsync(id);
            if (resp.Errors != null) return NotFound();
            await LoadViewBags();
            var dto = resp.Data as QuestionResponseDto;
            ViewBag.Subjects = dto != null && dto.grade_id > 0
                ? (await _subjectService.GetByGradeIdAsync(dto.grade_id)).Data
                : null;
            return View("~/Views/Question/Edit.cshtml", dto);
        }

        [HttpPost("question/edit/{id}")]
        [RequestFormLimits(ValueCountLimit = 2048)]
        public async Task<IActionResult> Edit(long id, QuestionResponseDto input, string listingPageUrl)
        {
            ViewBag.FormTitle = "Edit Question";
            var request = new QuestionRequestDto
            {
                subject_id = input.subject_id,
                grade_id = input.grade_id,
                question_type = input.question_type,
                question_text = input.question_text,
                question_html = input.question_html,
                image_url = input.image_url,
                hint = input.hint,
                explanation = input.explanation,
                difficulty = input.difficulty,
                default_marks = input.default_marks,
                negative_marks = input.negative_marks,
                is_active = input.is_active,
                eco_table_json = input.eco_table_json,
                tags_json = input.tags_json,
                answer_options = input.answer_options ?? new List<AnswerOptionDto>()
            };
            var resp = await _service.UpdateQuestionAsync(id, request);
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
                return View("~/Views/Question/Edit.cshtml", input);
            }
            SuccessMessage("Question updated successfully.");
            return !string.IsNullOrWhiteSpace(listingPageUrl) ? Redirect(listingPageUrl) : RedirectToAction("Index");
        }

        [HttpGet("question/delete/{id}")]
        public async Task<IActionResult> Delete(long id, int page_number = 1)
        {
            try
            {
                var resp = await _service.DeleteQuestionAsync(id);
                if (resp.IsSuccess) SuccessMessage("Question deleted successfully.");
                else if (resp.Errors != null) ErrorMessage(resp.Errors.Detail);
            }
            catch (NotFoundException nfex) { ErrorMessage(nfex.Message); }
            return RedirectToAction("Index", new { page_number });
        }

        [HttpPost("question/upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, error = "No file uploaded." });
            try
            {
                using var stream = file.OpenReadStream();
                var url = await _service.SaveQuestionImageAsync(stream, file.FileName);
                return Json(new { success = true, url });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("question/subjects-by-grade")]
        public async Task<IActionResult> SubjectsByGrade(long grade_id = 0)
        {
            var response = grade_id <= 0
                ? await _subjectService.GetAllForSelector()
                : await _subjectService.GetByGradeIdAsync(grade_id);
            return Json(response);
        }

        [HttpGet("question/bulk-import")]
        public async Task<IActionResult> BulkImport()
        {
            ViewBag.FormTitle = "Bulk Import Questions";
            await LoadViewBags();
            return View("~/Views/Question/BulkImport.cshtml");
        }

        [HttpPost("question/bulk-import")]
        public async Task<IActionResult> BulkImport(IFormFile file, long subject_id, long grade_id)
        {
            ViewBag.FormTitle = "Bulk Import Questions";
            await LoadViewBags();
            if (file == null || file.Length == 0)
            {
                ErrorMessage("Please upload an Excel file (.xlsx).");
                return View("~/Views/Question/BulkImport.cshtml");
            }
            try
            {
                using var stream = file.OpenReadStream();
                var result = await _service.BulkImportFromExcelAsync(stream, subject_id, grade_id);
                ViewBag.ImportResult = result;
                SuccessMessage($"Imported {result.ImportedCount} of {result.TotalRows} questions.");
            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message);
            }
            return View("~/Views/Question/BulkImport.cshtml");
        }

        [HttpGet("question/search")]
        public async Task<IActionResult> Search([FromQuery] QuestionSearchRequestDto request)
        {
            var resp = await _service.SearchQuestionsAsync(request);
            return Json(new { success = true, data = resp });
        }
    }
}
