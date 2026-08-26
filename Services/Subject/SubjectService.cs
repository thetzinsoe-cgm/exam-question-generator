using ExamSystem.Constraints;
using ExamSystem.DAO.Subject;
using ExamSystem.DTOs.Common;
using ExamSystem.DTOs.Subject;
using ExamSystem.Entity;
using ExamSystem.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Services.Subject
{
    public interface ISubjectService
    {
        Task<(List<SubjectResponseDto>, int)> GetSubjectsAsync(SubjectFilterDto filter);
        Task<Response> GetSubjectAsync(long id);
        Task<Response> CreateSubjectAsync(SubjectRequestDto dto);
        Task<Response> UpdateSubjectAsync(long id, SubjectRequestDto dto);
        Task<Response> DeleteSubjectAsync(long id);
        Task<Response> GetByGradeIdAsync(long gradeId);
        Task<Response> GetAllForSelector();
    }

    public class SubjectService : ISubjectService
    {
        private readonly ISubjectDao _dao;

        public SubjectService(ISubjectDao dao)
        {
            _dao = dao;
        }

        public async Task<(List<SubjectResponseDto>, int)> GetSubjectsAsync(SubjectFilterDto filter)
        {
            IQueryable<m_subject> query = _dao.GetAll().Include(s => s.grade);

            if (!string.IsNullOrWhiteSpace(filter.search))
            {
                var s = filter.search.Trim().ToLower();
                query = query.Where(x => x.name.ToLower().Contains(s)
                                      || (x.code != null && x.code.ToLower().Contains(s)));
            }
            if (filter.grade_id.HasValue)
            {
                query = query.Where(x => x.grade_id == filter.grade_id.Value);
            }
            if (filter.is_active.HasValue)
            {
                query = query.Where(x => x.is_active == filter.is_active.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.grade.sort_order)
                .ThenBy(x => x.name)
                .Skip((filter.page_number - 1) * filter.page_size)
                .Take(filter.page_size)
                .ToListAsync();

            var dtos = items.Select(s => new SubjectResponseDto
            {
                id = s.id,
                grade_id = s.grade_id,
                grade_name = s.grade?.name,
                name = s.name,
                code = s.code,
                description = s.description,
                total_marks = s.total_marks,
                pass_marks = s.pass_marks,
                duration_minutes = s.duration_minutes,
                is_active = s.is_active,
                question_count = s.questions?.Count ?? 0,
                created_datetime = s.created_datetime,
                updated_datetime = s.updated_datetime
            }).ToList();

            return (dtos, total);
        }

        public async Task<Response> GetSubjectAsync(long id)
        {
            var s = await _dao.GetById(id);
            if (s == null) return Response.Error(new Error { Status = 404, Title = "Not Found", Detail = "Subject not found." });
            return Response.Success(new SubjectResponseDto
            {
                id = s.id,
                grade_id = s.grade_id,
                grade_name = s.grade?.name,
                name = s.name,
                code = s.code,
                description = s.description,
                total_marks = s.total_marks,
                pass_marks = s.pass_marks,
                duration_minutes = s.duration_minutes,
                is_active = s.is_active,
                question_count = s.questions?.Count ?? 0
            });
        }

        public async Task<Response> CreateSubjectAsync(SubjectRequestDto dto)
        {
            if (await _dao.CodeExists(dto.code))
            {
                return Response.Error(new Error { Status = 409, Title = "Duplicate Code", Detail = "Subject code already exists." });
            }
            var entity = new m_subject
            {
                grade_id = dto.grade_id,
                name = dto.name,
                code = dto.code,
                description = dto.description,
                total_marks = dto.total_marks,
                pass_marks = dto.pass_marks,
                duration_minutes = dto.duration_minutes,
                is_active = dto.is_active,
                is_deleted = false,
                created_datetime = DateTime.Now,
                updated_datetime = DateTime.Now,
                created_user_id = AuthUser.Id,
                updated_user_id = AuthUser.Id
            };
            await _dao.Add(entity);
            return Response.Success(new { id = entity.id, name = entity.name });
        }

        public async Task<Response> UpdateSubjectAsync(long id, SubjectRequestDto dto)
        {
            var entity = await _dao.GetById(id);
            if (entity == null) throw new NotFoundException("Subject not found.");

            if (await _dao.CodeExists(dto.code, id))
            {
                return Response.Error(new Error { Status = 409, Title = "Duplicate Code", Detail = "Subject code already exists." });
            }

            entity.grade_id = dto.grade_id;
            entity.name = dto.name;
            entity.code = dto.code;
            entity.description = dto.description;
            entity.total_marks = dto.total_marks;
            entity.pass_marks = dto.pass_marks;
            entity.duration_minutes = dto.duration_minutes;
            entity.is_active = dto.is_active;
            entity.updated_datetime = DateTime.Now;
            entity.updated_user_id = AuthUser.Id;

            await _dao.Update(entity);
            return Response.Success(new { id = entity.id });
        }

        public async Task<Response> DeleteSubjectAsync(long id)
        {
            var entity = await _dao.GetById(id);
            if (entity == null) throw new NotFoundException("Subject not found.");

            if (entity.questions != null && entity.questions.Any(q => !q.is_deleted))
            {
                return Response.Error(new Error { Status = 409, Title = "Referenced", Detail = "Cannot delete subject with active questions." });
            }

            await _dao.Delete(entity);
            return Response.Success("Subject deleted successfully.");
        }

        public async Task<Response> GetByGradeIdAsync(long gradeId)
        {
            var query = _dao.GetAll().Include(s => s.grade).Where(s => s.is_active);
            if (gradeId > 0) query = query.Where(s => s.grade_id == gradeId);
            var list = await query.OrderBy(s => s.name)
                .Select(s => new SubjectResponseDto
                {
                    id = s.id,
                    grade_id = s.grade_id,
                    grade_name = s.grade.name,
                    name = s.name,
                    code = s.code,
                    total_marks = s.total_marks
                }).ToListAsync();
            return Response.Success(list);
        }

        public async Task<Response> GetAllForSelector()
        {
            var list = await _dao.GetAll()
                .Include(s => s.grade)
                .Where(s => s.is_active)
                .OrderBy(s => s.grade.level).ThenBy(s => s.name)
                .Select(s => new { s.id, s.name, s.code, grade_name = s.grade.name })
                .ToListAsync();
            return Response.Success(list);
        }
    }
}
