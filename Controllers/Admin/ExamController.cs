using ExamSystem.Attributes;
using ExamSystem.Constraints;
using ExamSystem.DTOs.Common;
using ExamSystem.DTOs.Exam;
using ExamSystem.Exceptions;
using ExamSystem.Helpers;
using ExamSystem.Services;
using ExamSystem.Services.Exam;
using ExamSystem.Services.Grade;
using ExamSystem.Services.MarkingRule;
using ExamSystem.Services.PdfRender;
using ExamSystem.Services.Subject;
using ExamSystem.Utilities;
using ExamSystem.ViewModels.Exam;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ExamSystem.Controllers.Admin
{
    [Route("admin")]
    [AuthorizeUser]
    public class ExamController : BaseController
    {
        private readonly IExamService _service;
        private readonly IGradeService _gradeService;
        private readonly ISubjectService _subjectService;
        private readonly IMarkingRuleService _markingRuleService;
        private readonly IViewRenderService _viewRender;
        private readonly SessionService _session;

        public ExamController(IExamService service, IGradeService gradeService, ISubjectService subjectService,
            IMarkingRuleService markingRuleService, IViewRenderService viewRender, SessionService sessionService)
        {
            _service = service;
            _gradeService = gradeService;
            _subjectService = subjectService;
            _markingRuleService = markingRuleService;
            _viewRender = viewRender;
            _session = sessionService;
        }

        private async Task LoadViewBags()
        {
            ViewBag.Grades = (await _gradeService.GetAllForDropdownAsync()).Data;
        }

        [HttpGet("exam/index")]
        public async Task<IActionResult> Index([FromQuery] ExamFilterDto filter)
        {
            filter ??= new ExamFilterDto();
            var (list, total) = await _service.GetExamsAsync(filter);
            var paginated = PaginationHelper.Paginated(list, filter.page_number, filter.page_size, total, HttpContext.Request);
            await LoadViewBags();
            ViewBag.Subjects = filter.grade_id.HasValue
                ? (await _subjectService.GetByGradeIdAsync(filter.grade_id.Value)).Data
                : null;

            var vm = new ExamIndexViewModel
            {
                Filter = filter,
                PageSizeOptions = DropDownHelper.GetPageSizeOptions(filter.page_size),
                Response = paginated
            };
            return View("~/Views/Exam/Index.cshtml", vm);
        }

        [HttpGet("exam/generate")]
        public async Task<IActionResult> Generate()
        {
            ViewBag.FormTitle = "Generate New Exam";
            ViewBag.QuestionTypes = ExamSystem.Constraints.QuestionTypes.AllTypes()
                .Select(t => new { id = t, name = t.GetTypeName() })
                .ToList();
            await LoadViewBags();
            return View("~/Views/Exam/Generate.cshtml");
        }

        [HttpPost("exam/generate")]
        [RequestFormLimits(ValueCountLimit = 4096)]
        public async Task<IActionResult> Generate(ExamGenerateRequestDto request)
        {
            ViewBag.FormTitle = "Generate New Exam";
            if (!ModelState.IsValid) { await LoadViewBags(); return View("~/Views/Exam/Generate.cshtml", request); }
            var resp = await _service.GenerateExamAsync(request);
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
                return View("~/Views/Exam/Generate.cshtml", request);
            }
            SuccessMessage("Exam generated successfully.");
            dynamic meta = resp.Meta ?? new { };
            return RedirectToAction("Preview", new { id = ((dynamic)resp.Data).id });
        }

        [HttpPost("exam/generate-manual")]
        [RequestFormLimits(ValueCountLimit = 8192)]
        public async Task<IActionResult> GenerateManual([FromBody] ManualExamGenerateRequestDto request)
        {
            if (!ModelState.IsValid) 
            { 
                await LoadViewBags(); 
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) }); 
            }
            var resp = await _service.GenerateExamManualAsync(request);
            if (!resp.IsSuccess)
            {
                return Json(new { success = false, error = resp.Errors?.Detail ?? "Failed to generate exam." });
            }
            var data = (dynamic)resp.Data;
            return Json(new { success = true, id = data.id, exam_code = data.exam_code, redirect = $"/admin/exam/preview/{data.id}" });
        }

        [HttpGet("exam/preview/{id}")]
        public async Task<IActionResult> Preview(long id)
        {
            var resp = await _service.GetExamWithQuestionsAsync(id);
            if (resp.Errors != null) return NotFound();
            return View("~/Views/Exam/Preview.cshtml", resp.Data as ExamResponseDto);
        }

        [HttpGet("exam/edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            ViewBag.FormTitle = "Edit Exam";
            ViewBag.QuestionTypes = ExamSystem.Constraints.QuestionTypes.AllTypes()
                .Select(t => new { id = t, name = t.GetTypeName() })
                .ToList();
            var resp = await _service.GetExamWithQuestionsAsync(id);
            if (resp.Errors != null) return NotFound();
            var dto = resp.Data as ExamResponseDto;
            await LoadViewBags();
            var subjectResp = await _subjectService.GetByGradeIdAsync(dto.grade_id);
            ViewBag.Subjects = subjectResp.Data;
            return View("~/Views/Exam/Edit.cshtml", dto);
        }

        [HttpPost("exam/edit/{id}")]
        public async Task<IActionResult> Edit(long id, ExamResponseDto input, string listingPageUrl)
        {
            ViewBag.FormTitle = "Edit Exam";
            var resp = await _service.UpdateExamAsync(id, input);
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
                ViewBag.Subjects = (await _subjectService.GetByGradeIdAsync(input.grade_id)).Data;
                return View("~/Views/Exam/Edit.cshtml", input);
            }
            SuccessMessage("Exam updated successfully.");
            return !string.IsNullOrWhiteSpace(listingPageUrl) ? Redirect(listingPageUrl) : RedirectToAction("Index");
        }

        [HttpPost("exam/edit-questions/{id}")]
        [RequestFormLimits(ValueCountLimit = 8192)]
        public async Task<IActionResult> EditQuestions(long id, [FromBody] ManualExamGenerateRequestDto request)
        {
            var resp = await _service.UpdateExamManualAsync(id, request);
            if (!resp.IsSuccess)
            {
                return Json(new { success = false, error = resp.Errors?.Detail ?? "Failed to update exam." });
            }
            var data = (dynamic)resp.Data;
            return Json(new { success = true, id = data.id, total_questions = data.total_questions, total_marks = data.total_marks, redirect = $"/admin/exam/preview/{data.id}" });
        }

        [HttpGet("exam/delete/{id}")]
        public async Task<IActionResult> Delete(long id, int page_number = 1)
        {
            try
            {
                var resp = await _service.DeleteExamAsync(id);
                if (resp.IsSuccess) SuccessMessage("Exam deleted successfully.");
                else if (resp.Errors != null) ErrorMessage(resp.Errors.Detail);
            }
            catch (NotFoundException nfex) { ErrorMessage(nfex.Message); }
            return RedirectToAction("Index", new { page_number });
        }

        [HttpGet("exam/export-pdf/{id}")]
        public async Task<IActionResult> ExportPdf(long id)
        {
            var examResp = await _service.GetExamWithQuestionsAsync(id);
            if (examResp.Errors != null) return NotFound();
            var exam = examResp.Data as ExamResponseDto;

            string html;
            try
            {
                html = await _viewRender.RenderToStringAsync(this, "~/Views/Exam/PrintTemplate.cshtml", exam);
            }
            catch
            {
                html = FallbackPrintHtml(exam);
            }

            var pdf = await _service.ExportToPdfAsync(id, html);
            Response.Headers["Content-Disposition"] = $"inline; filename=Exam_{exam?.exam_code ?? id.ToString()}.pdf";
            return File(pdf, "application/pdf");
        }

        [HttpGet("marking-rules-by-subject")]
        public async Task<IActionResult> GetMarkingRulesBySubject(long subjectId)
        {
            var resp = await _markingRuleService.GetBySubjectIdAsync(subjectId);
            return Json(resp.Data);
        }

        [HttpGet("exam/subjects-by-grade")]
        public async Task<IActionResult> SubjectsByGrade(long grade_id = 0)
        {
            var response = grade_id <= 0
                ? await _subjectService.GetAllForSelector()
                : await _subjectService.GetByGradeIdAsync(grade_id);
            return Json(response);
        }

        private static string FallbackPrintHtml(ExamResponseDto exam)
        {
            var sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html><head><meta charset='utf-8'/>");
            sb.Append("<title>").Append(System.Net.WebUtility.HtmlEncode(exam?.title ?? "Exam")).Append("</title>");
            sb.Append(@"
<style>
body{font-family:Arial,sans-serif;margin:20px;color:#111}
h1{text-align:center;border-bottom:2px solid #333;padding-bottom:8px}
.meta{display:flex;justify-content:space-between;margin-bottom:24px;font-size:14px}
.q{margin-bottom:18px;padding:10px;border-bottom:1px dashed #ccc}
.qn{font-weight:bold;margin-right:6px}
.opt{margin-left:24px;margin-top:4px}
table{border-collapse:collapse;width:100%;margin:8px 0}
td,th{border:1px solid #999;padding:6px}
img{max-width:100%;height:auto;margin:6px 0}
</style></head><body>");
            sb.Append("<h1>").Append(System.Net.WebUtility.HtmlEncode(exam?.title)).Append("</h1>");
            sb.Append("<div class='meta'>");
            sb.Append("<span>Exam Code: <b>").Append(System.Net.WebUtility.HtmlEncode(exam?.exam_code)).Append("</b></span>");
            sb.Append("<span>Subject: <b>").Append(System.Net.WebUtility.HtmlEncode(exam?.subject_name)).Append("</b> | Grade: <b>").Append(System.Net.WebUtility.HtmlEncode(exam?.grade_name)).Append("</b></span>");
            sb.Append("<span>Duration: <b>").Append(exam?.duration_minutes.ToString()).Append(" min</b> | Marks: <b>").Append(exam?.total_marks.ToString("0.##")).Append("</b></span>");
            sb.Append("</div>");
            sb.Append("<p>").Append(System.Net.WebUtility.HtmlEncode(exam?.description)).Append("</p>");
            sb.Append("<div id='questions'>");

            var questions = (exam?.questions ?? new List<ExamQuestionDto>()).OrderBy(q => q.question_number).ToList();
            for (var i = 0; i < questions.Count; i++)
            {
                var q = questions[i];
                var qNum = (i + 1).ToString();
                sb.Append("<div class='q'>");
                sb.Append("<span class='qn'>").Append(qNum).Append(".</span> (").Append(q.marks_allocated.ToString("0.##")).Append(" marks) ").Append(System.Net.WebUtility.HtmlEncode(q.question_type_name));
                sb.Append("<div style='margin-left:22px'>");
                if (!string.IsNullOrWhiteSpace(q.image_url))
                    sb.Append("<img src='").Append(System.Net.WebUtility.HtmlEncode(q.image_url)).Append("' alt=''/>");
                sb.Append("<div>").Append(System.Net.WebUtility.HtmlEncode(q.question_text)).Append("</div>");
                if (!string.IsNullOrWhiteSpace(q.eco_table_json))
                    sb.Append(RenderTable(q.eco_table_json));

                if (q.answer_options != null && q.answer_options.Any())
                {
                    var idx = 0;
                    foreach (var a in q.answer_options.OrderBy(a => a.sort_order))
                    {
                        var letter = ((char)('A' + idx)).ToString();
                        sb.Append("<div class='opt'>").Append(letter).Append(". ").Append(System.Net.WebUtility.HtmlEncode(a.option_text)).Append("</div>");
                        idx++;
                    }
                }
                sb.Append("</div></div>");
            }
            sb.Append("</div></body></html>");
            return sb.ToString();
        }

        private static string RenderTable(string ecoTableJson)
        {
            try
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(ecoTableJson);
                if (obj == null || !obj.Any()) return "";
                var sb = new StringBuilder();
                sb.Append("<table>");
                foreach (var r in obj)
                {
                    sb.Append("<tr>");
                    foreach (var c in r)
                        sb.Append("<td>").Append(System.Net.WebUtility.HtmlEncode(c)).Append("</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
