using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using ExamSystem.Constraints;
using ExamSystem.DAO.Exam;
using ExamSystem.DAO.ExamQuestion;
using ExamSystem.DAO.MarkingRule;
using ExamSystem.DAO.Question;
using ExamSystem.DTOs.Common;
using ExamSystem.DTOs.Exam;
using ExamSystem.DTOs.Question;
using ExamSystem.Entity;
using ExamSystem.Exceptions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ExamSystem.Services.Exam
{
    public interface IExamService
    {
        Task<(List<ExamResponseDto>, int)> GetExamsAsync(ExamFilterDto filter);
        Task<Response> GetExamAsync(long id);
        Task<Response> GetExamWithQuestionsAsync(long id);
        Task<Response> GenerateExamAsync(ExamGenerateRequestDto dto);
        Task<Response> GenerateExamManualAsync(ManualExamGenerateRequestDto dto);
        Task<Response> UpdateExamManualAsync(long id, ManualExamGenerateRequestDto dto);
        Task<Response> DeleteExamAsync(long id);
        Task<Response> UpdateExamAsync(long id, ExamResponseDto dto);
        Task<byte[]> ExportToPdfAsync(long examId, string printTemplateHtml);
    }

    public class ExamService : IExamService
    {
        private readonly IExamDao _examDao;
        private readonly IExamQuestionDao _examQuestionDao;
        private readonly IQuestionDao _questionDao;
        private readonly IMarkingRuleDao _markingRuleDao;
        private readonly IConverter _pdfConverter;

        public ExamService(IExamDao examDao, IExamQuestionDao examQuestionDao,
            IQuestionDao questionDao, IMarkingRuleDao markingRuleDao, IConverter pdfConverter)
        {
            _examDao = examDao;
            _examQuestionDao = examQuestionDao;
            _questionDao = questionDao;
            _markingRuleDao = markingRuleDao;
            _pdfConverter = pdfConverter;
        }

        public async Task<(List<ExamResponseDto>, int)> GetExamsAsync(ExamFilterDto filter)
        {
            IQueryable<t_exam> query = _examDao.GetAll().Include(e => e.subject).Include(e => e.grade);
            if (!string.IsNullOrWhiteSpace(filter.search))
            {
                var s = filter.search.Trim().ToLower();
                query = query.Where(e => e.title.ToLower().Contains(s)
                                      || (e.exam_code != null && e.exam_code.ToLower().Contains(s)));
            }
            if (filter.subject_id.HasValue) query = query.Where(e => e.subject_id == filter.subject_id.Value);
            if (filter.grade_id.HasValue) query = query.Where(e => e.grade_id == filter.grade_id.Value);
            if (filter.is_active.HasValue) query = query.Where(e => e.is_active == filter.is_active.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.id)
                .Skip((filter.page_number - 1) * filter.page_size)
                .Take(filter.page_size)
                .ToListAsync();

            var dtos = items.Select(e => MapExam(e)).ToList();
            return (dtos, total);
        }

        public async Task<Response> GetExamAsync(long id)
        {
            var e = await _examDao.GetById(id);
            if (e == null) return Response.Error(new Error { Status = 404, Title = "Not Found", Detail = "Exam not found." });
            return Response.Success(MapExam(e));
        }

        public async Task<Response> GetExamWithQuestionsAsync(long id)
        {
            var e = await _examDao.GetByIdWithQuestions(id);
            if (e == null) return Response.Error(new Error { Status = 404, Title = "Not Found", Detail = "Exam not found." });
            var dto = MapExam(e);
            dto.questions = e.exam_questions?
                .OrderBy(eq => eq.question_number)
                .Select(eq => new ExamQuestionDto
                {
                    exam_question_id = eq.id,
                    question_id = eq.question_id,
                    question_number = eq.question_number,
                    section_name = eq.section_name,
                    marks_allocated = eq.marks_allocated,
                    question_type = eq.question?.question_type ?? 0,
                    question_type_name = (eq.question?.question_type ?? 0).GetTypeName(),
                    question_text = eq.question?.question_text,
                    question_html = eq.question?.question_html,
                    image_url = eq.question?.image_url,
                    eco_table_json = eq.question?.eco_table_json,
                    answer_options = eq.question?.answer_options?
                        .Select(a => new AnswerOptionDto
                        {
                            id = a.id,
                            option_text = a.option_text,
                            option_html = a.option_html,
                            option_image_url = a.option_image_url,
                            sort_order = a.sort_order
                        }).ToList() ?? new List<AnswerOptionDto>()
                }).ToList() ?? new List<ExamQuestionDto>();
            return Response.Success(dto);
        }

        public async Task<Response> GenerateExamAsync(ExamGenerateRequestDto dto)
        {
            var examCode = $"EX-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
            while (await _examDao.ExamCodeExists(examCode))
            {
                examCode = $"EX-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
            }

            var exam = new t_exam
            {
                exam_code = examCode,
                title = dto.title,
                subject_id = dto.subject_id,
                grade_id = dto.grade_id,
                total_questions = 0,
                duration_minutes = dto.duration_minutes > 0 ? dto.duration_minutes : Consts.DefaultDurationMinutes,
                total_marks = 0,
                pass_marks = dto.pass_marks,
                exam_date = dto.exam_date,
                description = dto.description,
                exam_config_json = JsonConvert.SerializeObject(dto.sections),
                is_active = true,
                is_deleted = false,
                created_datetime = DateTime.Now,
                updated_datetime = DateTime.Now,
                created_user_id = AuthUser.Id,
                updated_user_id = AuthUser.Id
            };
            await _examDao.Add(exam);

            var sections = dto.sections != null && dto.sections.Any()
                ? dto.sections
                : BuildSectionsFromRules(dto);

            var allExamQuestions = new List<t_exam_question>();
            var qNumber = 1;
            decimal totalMarks = 0;

            foreach (var section in sections)
            {
                var questions = await _questionDao.GetRandomQuestionsBySubjectAndType(
                    dto.subject_id,
                    section.question_type,
                    section.difficulty,
                    section.question_count);

                foreach (var q in questions)
                {
                    var marks = section.marks_per_question > 0 ? section.marks_per_question : q.default_marks;
                    allExamQuestions.Add(new t_exam_question
                    {
                        exam_id = exam.id,
                        question_id = q.id,
                        question_number = qNumber++,
                        marks_allocated = marks,
                        section_name = section.section_name,
                        is_deleted = false,
                        created_datetime = DateTime.Now
                    });
                    totalMarks += marks;
                }
            }

            if (dto.randomize_questions)
            {
                allExamQuestions = allExamQuestions
                    .OrderBy(_ => Guid.NewGuid())
                    .ToList();
                qNumber = 1;
                foreach (var eq in allExamQuestions) eq.question_number = qNumber++;
            }

            exam.total_questions = allExamQuestions.Count;
            exam.total_marks = totalMarks;
            await _examDao.Update(exam);
            await _examQuestionDao.AddRange(allExamQuestions);

            return Response.Success(new { id = exam.id, exam_code = exam.exam_code, total_questions = exam.total_questions, total_marks = exam.total_marks });
        }

        public async Task<Response> GenerateExamManualAsync(ManualExamGenerateRequestDto dto)
        {
            if (dto.sections == null || !dto.sections.Any())
            {
                return Response.Error(new Error { Detail = "At least one section is required." });
            }

            var allQuestionIds = dto.sections
                .SelectMany(s => s.questions)
                .Select(q => q.question_id)
                .ToList();

            if (!allQuestionIds.Any())
            {
                return Response.Error(new Error { Detail = "At least one question must be selected." });
            }

            if (allQuestionIds.Distinct().Count() != allQuestionIds.Count)
            {
                return Response.Error(new Error { Detail = "Duplicate questions are not allowed." });
            }

            var questions = await _questionDao.GetByIdsAsync(allQuestionIds);
            if (questions.Count != allQuestionIds.Count)
            {
                return Response.Error(new Error { Detail = "One or more selected questions not found." });
            }

            var invalidQuestions = questions.Where(q => q.subject_id != dto.subject_id || q.grade_id != dto.grade_id).ToList();
            if (invalidQuestions.Any())
            {
                return Response.Error(new Error { Detail = "Some questions do not belong to the selected subject/grade." });
            }

            var examCode = $"EX-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
            while (await _examDao.ExamCodeExists(examCode))
            {
                examCode = $"EX-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
            }

            var exam = new t_exam
            {
                exam_code = examCode,
                title = dto.title,
                subject_id = dto.subject_id,
                grade_id = dto.grade_id,
                total_questions = 0,
                duration_minutes = dto.duration_minutes > 0 ? dto.duration_minutes : Consts.DefaultDurationMinutes,
                total_marks = 0,
                pass_marks = dto.pass_marks,
                exam_date = dto.exam_date,
                description = dto.description,
                exam_config_json = JsonConvert.SerializeObject(dto.sections),
                is_active = true,
                is_deleted = false,
                created_datetime = DateTime.Now,
                updated_datetime = DateTime.Now,
                created_user_id = AuthUser.Id,
                updated_user_id = AuthUser.Id
            };
            await _examDao.Add(exam);

            var allExamQuestions = new List<t_exam_question>();
            decimal totalMarks = 0;

            foreach (var section in dto.sections)
            {
                foreach (var q in section.questions.OrderBy(x => x.question_number))
                {
                    var questionEntity = questions.First(x => x.id == q.question_id);
                    var marks = q.marks_allocated > 0 ? q.marks_allocated : questionEntity.default_marks;

                    allExamQuestions.Add(new t_exam_question
                    {
                        exam_id = exam.id,
                        question_id = q.question_id,
                        question_number = q.question_number,
                        marks_allocated = marks,
                        section_name = q.section_name ?? section.section_name,
                        is_deleted = false,
                        created_datetime = DateTime.Now
                    });
                    totalMarks += marks;
                }
            }

            exam.total_questions = allExamQuestions.Count;
            exam.total_marks = totalMarks;
            await _examDao.Update(exam);
            await _examQuestionDao.AddRange(allExamQuestions);

            return Response.Success(new { id = exam.id, exam_code = exam.exam_code, total_questions = exam.total_questions, total_marks = exam.total_marks });
        }

        public async Task<Response> DeleteExamAsync(long id)
        {
            var e = await _examDao.GetById(id);
            if (e == null) throw new NotFoundException("Exam not found.");
            await _examDao.Delete(e);
            return Response.Success("Exam deleted successfully.");
        }

        public async Task<Response> UpdateExamAsync(long id, ExamResponseDto dto)
        {
            var e = await _examDao.GetById(id);
            if (e == null) return Response.Error(new Error { Status = 404, Title = "Not Found", Detail = "Exam not found." });

            e.title = dto.title;
            e.description = dto.description;
            e.grade_id = dto.grade_id;
            e.subject_id = dto.subject_id;
            e.duration_minutes = dto.duration_minutes;
            e.pass_marks = dto.pass_marks;
            e.exam_date = dto.exam_date;
            e.is_active = dto.is_active;
            e.updated_datetime = DateTime.Now;
            e.updated_user_id = AuthUser.Id;

            await _examDao.Update(e);
            return Response.Success("Exam updated successfully.");
        }

        public async Task<Response> UpdateExamManualAsync(long id, ManualExamGenerateRequestDto dto)
        {
            var e = await _examDao.GetById(id);
            if (e == null) return Response.Error(new Error { Status = 404, Title = "Not Found", Detail = "Exam not found." });

            if (dto.sections == null || !dto.sections.Any())
            {
                return Response.Error(new Error { Detail = "At least one section is required." });
            }

            var allQuestionIds = dto.sections
                .SelectMany(s => s.questions)
                .Select(q => q.question_id)
                .ToList();

            if (!allQuestionIds.Any())
            {
                return Response.Error(new Error { Detail = "At least one question must be selected." });
            }

            if (allQuestionIds.Distinct().Count() != allQuestionIds.Count)
            {
                return Response.Error(new Error { Detail = "Duplicate questions are not allowed." });
            }

            var questions = await _questionDao.GetByIdsAsync(allQuestionIds);
            if (questions.Count != allQuestionIds.Count)
            {
                return Response.Error(new Error { Detail = "One or more selected questions not found." });
            }

            var invalidQuestions = questions.Where(q => q.subject_id != dto.subject_id || q.grade_id != dto.grade_id).ToList();
            if (invalidQuestions.Any())
            {
                return Response.Error(new Error { Detail = "Some questions do not belong to the selected subject/grade." });
            }

            e.title = dto.title;
            e.description = dto.description;
            e.grade_id = dto.grade_id;
            e.subject_id = dto.subject_id;
            e.duration_minutes = dto.duration_minutes > 0 ? dto.duration_minutes : e.duration_minutes;
            e.pass_marks = dto.pass_marks;
            e.exam_date = dto.exam_date;
            e.exam_config_json = JsonConvert.SerializeObject(dto.sections);
            e.updated_datetime = DateTime.Now;
            e.updated_user_id = AuthUser.Id;

            await _examDao.Update(e);

            await _examQuestionDao.DeleteByExamId(e.id);

            var allExamQuestions = new List<t_exam_question>();
            decimal totalMarks = 0;

            foreach (var section in dto.sections)
            {
                foreach (var q in section.questions.OrderBy(x => x.question_number))
                {
                    var questionEntity = questions.First(x => x.id == q.question_id);
                    var marks = q.marks_allocated > 0 ? q.marks_allocated : questionEntity.default_marks;

                    allExamQuestions.Add(new t_exam_question
                    {
                        exam_id = e.id,
                        question_id = q.question_id,
                        question_number = q.question_number,
                        marks_allocated = marks,
                        section_name = q.section_name ?? section.section_name,
                        is_deleted = false,
                        created_datetime = DateTime.Now
                    });
                    totalMarks += marks;
                }
            }

            e.total_questions = allExamQuestions.Count;
            e.total_marks = totalMarks;
            await _examDao.Update(e);
            await _examQuestionDao.AddRange(allExamQuestions);

            return Response.Success(new { id = e.id, total_questions = e.total_questions, total_marks = e.total_marks });
        }

        public async Task<byte[]> ExportToPdfAsync(long examId, string printTemplateHtml)
        {
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings
                    {
                        Top = 15, Bottom = 18, Left = 15, Right = 15, Unit = Unit.Millimeters
                    },
                    DPI = 300,
                    DocumentTitle = $"Exam_{examId}"
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = printTemplateHtml,
                        WebSettings =
                        {
                            DefaultEncoding = "utf-8",
                            PrintMediaType = true,
                            EnableJavascript = false,
                            LoadImages = true
                        },
                        HeaderSettings =
                        {
                            FontSize = 8,
                            FontName = "Noto Sans Myanmar, Pyidaungsu, Arial",
                            Left = "ExamSystem",
                            Line = false,
                            Spacing = 4
                        },
                        FooterSettings =
                        {
                            FontSize = 8,
                            FontName = "Noto Sans Myanmar, Pyidaungsu, Arial",
                            Line = true,
                            Center = "[page] / [toPage]",
                            Spacing = 4
                        }
                    }
                }
            };
            return await Task.FromResult(_pdfConverter.Convert(doc));
        }

        private List<ExamSectionDto> BuildSectionsFromRules(ExamGenerateRequestDto dto)
        {
            var rules = _markingRuleDao.GetBySubjectId(dto.subject_id).Result;
            if (rules == null || !rules.Any())
            {
                return new List<ExamSectionDto>
                {
                    new ExamSectionDto
                    {
                        section_name = "Section A",
                        question_type = QuestionTypes.MultipleChoice,
                        question_count = dto.total_questions > 0 ? dto.total_questions : Consts.DefaultQuestionsPerExam,
                        marks_per_question = 1
                    }
                };
            }

            var sections = new List<ExamSectionDto>();
            int seq = 0;
            foreach (var r in rules)
            {
                seq++;
                var qCount = r.max_questions > 0 ? r.max_questions : (dto.total_questions / rules.Count);
                sections.Add(new ExamSectionDto
                {
                    section_name = $"Section {((char)('A' + seq - 1)).ToString()}",
                    question_type = r.question_type,
                    difficulty = r.difficulty,
                    question_count = qCount,
                    marks_per_question = r.marks_per_question
                });
            }
            return sections;
        }

        private static ExamResponseDto MapExam(t_exam e)
        {
            return new ExamResponseDto
            {
                id = e.id,
                exam_code = e.exam_code,
                title = e.title,
                subject_id = e.subject_id,
                subject_name = e.subject?.name,
                grade_id = e.grade_id,
                grade_name = e.grade?.name,
                total_questions = e.total_questions,
                duration_minutes = e.duration_minutes,
                total_marks = e.total_marks,
                pass_marks = e.pass_marks,
                exam_date = e.exam_date,
                description = e.description,
                is_active = e.is_active,
                created_datetime = e.created_datetime,
                updated_datetime = e.updated_datetime
            };
        }
    }
}
