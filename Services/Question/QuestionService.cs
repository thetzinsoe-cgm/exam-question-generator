using ClosedXML.Excel;
using ExamSystem.Constraints;
using ExamSystem.DAO.Answer;
using ExamSystem.DAO.Question;
using ExamSystem.DTOs.Common;
using ExamSystem.DTOs.Question;
using ExamSystem.Entity;
using ExamSystem.Exceptions;
using ExamSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ExamSystem.Services.Question
{
    public interface IQuestionService
    {
        Task<(List<QuestionResponseDto>, int)> GetQuestionsAsync(QuestionFilterDto filter);
        Task<Response> GetQuestionAsync(long id);
        Task<Response> CreateQuestionAsync(QuestionRequestDto dto);
        Task<Response> UpdateQuestionAsync(long id, QuestionRequestDto dto);
        Task<Response> DeleteQuestionAsync(long id);
        Task<string> SaveQuestionImageAsync(Stream stream, string fileName);
        Task<BulkImportResultDto> BulkImportFromExcelAsync(Stream excelStream, long subjectId, long gradeId);
    }

    public class QuestionService : IQuestionService
    {
        private readonly IQuestionDao _dao;
        private readonly IAnswerDao _answerDao;
        private readonly FilePathHelper _filePathHelper;

        public QuestionService(IQuestionDao dao, IAnswerDao answerDao, FilePathHelper filePathHelper)
        {
            _dao = dao;
            _answerDao = answerDao;
            _filePathHelper = filePathHelper;
        }

        public async Task<(List<QuestionResponseDto>, int)> GetQuestionsAsync(QuestionFilterDto filter)
        {
            IQueryable<m_question> query = _dao.GetAll()
                .Include(q => q.subject)
                .Include(q => q.grade);

            if (!string.IsNullOrWhiteSpace(filter.search))
            {
                var s = filter.search.Trim().ToLower();
                query = query.Where(q => q.question_text.ToLower().Contains(s));
            }
            if (filter.subject_id.HasValue) query = query.Where(q => q.subject_id == filter.subject_id.Value);
            if (filter.grade_id.HasValue) query = query.Where(q => q.grade_id == filter.grade_id.Value);
            if (filter.question_type.HasValue) query = query.Where(q => q.question_type == filter.question_type.Value);
            if (filter.difficulty.HasValue) query = query.Where(q => q.difficulty == filter.difficulty.Value);
            if (filter.is_active.HasValue) query = query.Where(q => q.is_active == filter.is_active.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(q => q.id)
                .Skip((filter.page_number - 1) * filter.page_size)
                .Take(filter.page_size)
                .ToListAsync();

            var dtos = items.Select(q => Map(q)).ToList();
            return (dtos, total);
        }

        public async Task<Response> GetQuestionAsync(long id)
        {
            var q = await _dao.GetByIdWithAnswers(id);
            if (q == null) return Response.Error(new Error { Status = 404, Title = "Not Found", Detail = "Question not found." });
            return Response.Success(Map(q));
        }

        public async Task<Response> CreateQuestionAsync(QuestionRequestDto dto)
        {
            var entity = new m_question
            {
                subject_id = dto.subject_id,
                grade_id = dto.grade_id,
                question_type = dto.question_type,
                question_text = dto.question_text,
                question_html = dto.question_html,
                image_url = dto.image_url,
                hint = dto.hint,
                explanation = dto.explanation,
                difficulty = dto.difficulty,
                default_marks = dto.default_marks,
                negative_marks = dto.negative_marks,
                is_active = dto.is_active,
                is_deleted = false,
                eco_table_json = dto.eco_table_json,
                tags_json = dto.tags_json,
                created_datetime = DateTime.Now,
                updated_datetime = DateTime.Now,
                created_user_id = AuthUser.Id,
                updated_user_id = AuthUser.Id
            };

            await _dao.Add(entity);

            if (dto.answer_options != null && dto.answer_options.Any())
            {
                var options = dto.answer_options
                    .Where(o => !o.is_deleted)
                    .Select((o, i) => new m_answer_option
                    {
                        question_id = entity.id,
                        option_text = o.option_text,
                        option_html = o.option_html,
                        option_image_url = o.option_image_url,
                        is_correct = o.is_correct,
                        marks_allocated = o.marks_allocated,
                        sort_order = o.sort_order > 0 ? o.sort_order : i + 1,
                        is_deleted = false,
                        created_datetime = DateTime.Now,
                        updated_datetime = DateTime.Now
                    }).ToList();
                await _answerDao.AddRange(options);
            }

            return Response.Success(new { id = entity.id });
        }

        public async Task<Response> UpdateQuestionAsync(long id, QuestionRequestDto dto)
        {
            var entity = await _dao.GetByIdWithAnswers(id);
            if (entity == null) throw new NotFoundException("Question not found.");

            entity.subject_id = dto.subject_id;
            entity.grade_id = dto.grade_id;
            entity.question_type = dto.question_type;
            entity.question_text = dto.question_text;
            entity.question_html = dto.question_html;
            if (!string.IsNullOrWhiteSpace(dto.image_url)) entity.image_url = dto.image_url;
            entity.hint = dto.hint;
            entity.explanation = dto.explanation;
            entity.difficulty = dto.difficulty;
            entity.default_marks = dto.default_marks;
            entity.negative_marks = dto.negative_marks;
            entity.is_active = dto.is_active;
            entity.eco_table_json = dto.eco_table_json;
            entity.tags_json = dto.tags_json;
            entity.updated_datetime = DateTime.Now;
            entity.updated_user_id = AuthUser.Id;

            await _dao.Update(entity);

            if (dto.answer_options != null)
            {
                await _answerDao.DeleteByQuestionId(id);
                var options = dto.answer_options
                    .Where(o => !o.is_deleted)
                    .Select((o, i) => new m_answer_option
                    {
                        question_id = entity.id,
                        option_text = o.option_text,
                        option_html = o.option_html,
                        option_image_url = o.option_image_url,
                        is_correct = o.is_correct,
                        marks_allocated = o.marks_allocated,
                        sort_order = o.sort_order > 0 ? o.sort_order : i + 1,
                        is_deleted = false,
                        created_datetime = DateTime.Now,
                        updated_datetime = DateTime.Now
                    }).ToList();
                await _answerDao.AddRange(options);
            }

            return Response.Success(new { id = entity.id });
        }

        public async Task<Response> DeleteQuestionAsync(long id)
        {
            var entity = await _dao.GetById(id);
            if (entity == null) throw new NotFoundException("Question not found.");
            await _dao.Delete(entity);
            return Response.Success("Question deleted successfully.");
        }

        public async Task<string> SaveQuestionImageAsync(Stream stream, string fileName)
        {
            if (!FileUtility.IsAllowedImageExtension(fileName))
                throw new ArgumentException("Invalid image format.");

            var safeName = FileUtility.SanitizeFileName(fileName);
            var folder = _filePathHelper.QuestionImagesFolder;
            var fullPath = Path.Combine(folder, safeName);

            using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }

            var fileInfo = new FileInfo(fullPath);
            if (!FileUtility.IsWithinSizeLimit(fileInfo.Length))
            {
                fileInfo.Delete();
                throw new ArgumentException($"Image too large. Max {Consts.MaxImageSizeMB}MB.");
            }

            return _filePathHelper.GetRelativePath(fullPath);
        }

        public async Task<BulkImportResultDto> BulkImportFromExcelAsync(Stream excelStream, long subjectId, long gradeId)
        {
            var result = new BulkImportResultDto();
            using var wb = new XLWorkbook(excelStream);
            var ws = wb.Worksheets.FirstOrDefault();
            if (ws == null) { result.Errors.Add("No worksheet found."); return result; }

            var rows = ws.RowsUsed().Skip(1).ToList();
            result.TotalRows = rows.Count;

            foreach (var row in rows)
            {
                try
                {
                    var questionText = row.Cell(1).GetString();
                    if (string.IsNullOrWhiteSpace(questionText)) continue;

                    var typeCell = TryParseInt(row.Cell(2).GetString(), 1);
                    var marks = TryParseDecimal(row.Cell(3).GetString(), 1);

                    var dto = new QuestionRequestDto
                    {
                        subject_id = subjectId,
                        grade_id = gradeId,
                        question_type = (short)typeCell,
                        question_text = questionText,
                        question_html = row.Cell(4).GetString(),
                        difficulty = (short)TryParseInt(row.Cell(5).GetString(), 2),
                        default_marks = marks,
                        is_active = true
                    };

                    for (int i = 6; i <= 13; i += 2)
                    {
                        var opt = row.Cell(i).GetString();
                        if (string.IsNullOrWhiteSpace(opt)) continue;
                        var isCorrect = TryParseBoolean(row.Cell(i + 1).GetString());
                        dto.answer_options.Add(new AnswerOptionDto
                        {
                            option_text = opt,
                            is_correct = isCorrect,
                            marks_allocated = isCorrect ? marks : 0
                        });
                    }

                    var resp = await CreateQuestionAsync(dto);
                    if (resp.IsSuccess) result.ImportedCount++;
                    else { result.FailedCount++; result.Errors.Add($"Row: {resp.Errors?.Detail ?? "Unknown"}"); }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"Row {row.RowNumber()}: {ex.Message}");
                }
            }
            return await Task.FromResult(result);
        }

        private static int TryParseInt(string value, int defaultValue)
        {
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        private static decimal TryParseDecimal(string value, decimal defaultValue)
        {
            return decimal.TryParse(value, out var result) ? result : defaultValue;
        }

        private static bool TryParseBoolean(string value)
        {
            if (bool.TryParse(value, out var result)) return result;
            if (int.TryParse(value, out var intVal)) return intVal != 0;
            var trimmed = (value ?? string.Empty).Trim().ToUpperInvariant();
            return trimmed == "Y" || trimmed == "YES" || trimmed == "T" || trimmed == "TRUE" || trimmed == "1";
        }

        private static QuestionResponseDto Map(m_question q)
        {
            return new QuestionResponseDto
            {
                id = q.id,
                subject_id = q.subject_id,
                subject_name = q.subject?.name,
                grade_id = q.grade_id,
                grade_name = q.grade?.name,
                question_type = q.question_type,
                question_type_name = q.question_type.GetTypeName(),
                question_text = q.question_text,
                question_html = q.question_html,
                image_url = q.image_url,
                hint = q.hint,
                explanation = q.explanation,
                difficulty = q.difficulty,
                default_marks = q.default_marks,
                negative_marks = q.negative_marks,
                is_active = q.is_active,
                eco_table_json = q.eco_table_json,
                tags_json = q.tags_json,
                created_datetime = q.created_datetime,
                updated_datetime = q.updated_datetime,
                answer_options = q.answer_options?.Select(a => new AnswerOptionDto
                {
                    id = a.id,
                    option_text = a.option_text,
                    option_html = a.option_html,
                    option_image_url = a.option_image_url,
                    is_correct = a.is_correct,
                    marks_allocated = a.marks_allocated,
                    sort_order = a.sort_order
                }).ToList() ?? new List<AnswerOptionDto>()
            };
        }
    }
}
