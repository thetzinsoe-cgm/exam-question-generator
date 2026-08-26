using ExamSystem.Constraints;
using ExamSystem.DAO.MarkingRule;
using ExamSystem.DTOs.Common;
using ExamSystem.DTOs.MarkingRule;
using ExamSystem.Entity;
using ExamSystem.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Services.MarkingRule
{
    public interface IMarkingRuleService
    {
        Task<(List<MarkingRuleResponseDto>, int)> GetRulesAsync(MarkingRuleFilterDto filter);
        Task<Response> GetRuleAsync(long id);
        Task<Response> CreateRuleAsync(MarkingRuleRequestDto dto);
        Task<Response> UpdateRuleAsync(long id, MarkingRuleRequestDto dto);
        Task<Response> DeleteRuleAsync(long id);
        Task<Response> GetBySubjectIdAsync(long subjectId);
    }

    public class MarkingRuleService : IMarkingRuleService
    {
        private readonly IMarkingRuleDao _dao;

        public MarkingRuleService(IMarkingRuleDao dao)
        {
            _dao = dao;
        }

        public async Task<(List<MarkingRuleResponseDto>, int)> GetRulesAsync(MarkingRuleFilterDto filter)
        {
            IQueryable<m_marking_rule> query = _dao.GetAll().Include(r => r.subject);
            if (filter.subject_id.HasValue) query = query.Where(r => r.subject_id == filter.subject_id.Value);
            if (filter.question_type.HasValue) query = query.Where(r => r.question_type == filter.question_type.Value);
            if (filter.is_active.HasValue) query = query.Where(r => r.is_active == filter.is_active.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.id)
                .Skip((filter.page_number - 1) * filter.page_size)
                .Take(filter.page_size)
                .ToListAsync();

            var dtos = items.Select(r => Map(r)).ToList();
            return (dtos, total);
        }

        public async Task<Response> GetRuleAsync(long id)
        {
            var r = await _dao.GetById(id);
            if (r == null) return Response.Error(new Error { Status = 404, Title = "Not Found", Detail = "Rule not found." });
            return Response.Success(Map(r));
        }

        public async Task<Response> CreateRuleAsync(MarkingRuleRequestDto dto)
        {
            var entity = new m_marking_rule
            {
                subject_id = dto.subject_id,
                question_type = dto.question_type,
                marks_per_question = dto.marks_per_question,
                negative_marks = dto.negative_marks,
                min_questions = dto.min_questions,
                max_questions = dto.max_questions,
                difficulty = dto.difficulty,
                rule_name = dto.rule_name,
                description = dto.description,
                is_active = dto.is_active,
                is_deleted = false,
                created_datetime = DateTime.Now,
                updated_datetime = DateTime.Now,
                created_user_id = AuthUser.Id,
                updated_user_id = AuthUser.Id
            };
            await _dao.Add(entity);
            return Response.Success(new { id = entity.id });
        }

        public async Task<Response> UpdateRuleAsync(long id, MarkingRuleRequestDto dto)
        {
            var entity = await _dao.GetById(id);
            if (entity == null) throw new NotFoundException("Rule not found.");

            entity.subject_id = dto.subject_id;
            entity.question_type = dto.question_type;
            entity.marks_per_question = dto.marks_per_question;
            entity.negative_marks = dto.negative_marks;
            entity.min_questions = dto.min_questions;
            entity.max_questions = dto.max_questions;
            entity.difficulty = dto.difficulty;
            entity.rule_name = dto.rule_name;
            entity.description = dto.description;
            entity.is_active = dto.is_active;
            entity.updated_datetime = DateTime.Now;
            entity.updated_user_id = AuthUser.Id;
            await _dao.Update(entity);
            return Response.Success(new { id = entity.id });
        }

        public async Task<Response> DeleteRuleAsync(long id)
        {
            var entity = await _dao.GetById(id);
            if (entity == null) throw new NotFoundException("Rule not found.");
            await _dao.Delete(entity);
            return Response.Success("Rule deleted successfully.");
        }

        public async Task<Response> GetBySubjectIdAsync(long subjectId)
        {
            var rules = await _dao.GetBySubjectId(subjectId);
            var dtos = rules.Select(r => Map(r)).ToList();
            return Response.Success(dtos);
        }

        private static MarkingRuleResponseDto Map(m_marking_rule r)
        {
            return new MarkingRuleResponseDto
            {
                id = r.id,
                subject_id = r.subject_id,
                subject_name = r.subject?.name,
                question_type = r.question_type,
                question_type_name = r.question_type.GetTypeName(),
                rule_name = r.rule_name,
                description = r.description,
                marks_per_question = r.marks_per_question,
                negative_marks = r.negative_marks,
                min_questions = r.min_questions,
                max_questions = r.max_questions,
                difficulty = r.difficulty,
                is_active = r.is_active,
                created_datetime = r.created_datetime,
                updated_datetime = r.updated_datetime
            };
        }
    }
}
