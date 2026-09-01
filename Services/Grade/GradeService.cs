using ExamSystem.Constraints;
using ExamSystem.DAO.Grade;
using ExamSystem.DTOs.Common;
using ExamSystem.DTOs.Grade;
using ExamSystem.Entity;
using ExamSystem.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Services.Grade
{
    public interface IGradeService
    {
        Task<(List<GradeResponseDto>, int)> GetGradesAsync(GradeFilterDto filter);
        Task<Response> GetGradeAsync(long id);
        Task<Response> CreateGradeAsync(GradeRequestDto dto);
        Task<Response> UpdateGradeAsync(long id, GradeRequestDto dto);
        Task<Response> DeleteGradeAsync(long id);
        Task<Response> GetAllForDropdownAsync();
    }

    public class GradeService : IGradeService
    {
        private readonly IGradeDao _dao;

        public GradeService(IGradeDao dao)
        {
            _dao = dao;
        }

        public async Task<(List<GradeResponseDto>, int)> GetGradesAsync(GradeFilterDto filter)
        {
            var query = _dao.GetAll();

            if (!string.IsNullOrWhiteSpace(filter.search))
            {
                var s = filter.search.Trim().ToLower();
                query = query.Where(g => g.name.ToLower().Contains(s)
                                      || (g.level != null && g.level.ToLower().Contains(s)));
            }
            if (filter.is_active.HasValue)
            {
                query = query.Where(g => g.is_active == filter.is_active.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .Include(g => g.subjects)
                .OrderBy(g => g.sort_order)
                .ThenByDescending(g => g.id)
                .Skip((filter.page_number - 1) * filter.page_size)
                .Take(filter.page_size)
                .ToListAsync();

            var dtos = items.Select(g => new GradeResponseDto
            {
                id = g.id,
                name = g.name,
                level = g.level,
                description = g.description,
                sort_order = g.sort_order,
                is_active = g.is_active,
                subject_count = g.subjects?.Count ?? 0,
                created_datetime = g.created_datetime,
                updated_datetime = g.updated_datetime
            }).ToList();

            return (dtos, total);
        }

        public async Task<Response> GetGradeAsync(long id)
        {
            var g = await _dao.GetById(id);
            if (g == null) return Response.Error(new Error { Status = 404, Title = "Not Found", Detail = $"Grade {id} not found." });
            return Response.Success(new GradeResponseDto
            {
                id = g.id,
                name = g.name,
                level = g.level,
                description = g.description,
                sort_order = g.sort_order,
                is_active = g.is_active,
                subject_count = g.subjects?.Count ?? 0
            });
        }

        public async Task<Response> CreateGradeAsync(GradeRequestDto dto)
        {
            if (await _dao.GradeNameExists(dto.name))
            {
                return Response.Error(new Error { Status = 409, Title = "Duplicate", Detail = "Grade name already exists." });
            }
            var entity = new m_grade
            {
                name = dto.name,
                level = dto.level,
                description = dto.description,
                sort_order = dto.sort_order,
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

        public async Task<Response> UpdateGradeAsync(long id, GradeRequestDto dto)
        {
            var entity = await _dao.GetById(id);
            if (entity == null) throw new NotFoundException("Grade not found.");

            if (await _dao.GradeNameExists(dto.name, id))
            {
                return Response.Error(new Error { Status = 409, Title = "Duplicate", Detail = "Grade name already exists." });
            }

            entity.name = dto.name;
            entity.level = dto.level;
            entity.description = dto.description;
            entity.sort_order = dto.sort_order;
            entity.is_active = dto.is_active;
            entity.updated_datetime = DateTime.Now;
            entity.updated_user_id = AuthUser.Id;

            await _dao.Update(entity);
            return Response.Success(new { id = entity.id });
        }

        public async Task<Response> DeleteGradeAsync(long id)
        {
            var entity = await _dao.GetById(id);
            if (entity == null) throw new NotFoundException("Grade not found.");

            if (entity.subjects != null && entity.subjects.Any(s => !s.is_deleted))
            {
                return Response.Error(new Error { Status = 409, Title = "Referenced", Detail = "Cannot delete grade with active subjects." });
            }

            await _dao.Delete(entity);
            return Response.Success("Grade deleted successfully.");
        }

        public async Task<Response> GetAllForDropdownAsync()
        {
            var list = await _dao.GetAll()
                .Where(g => g.is_active)
                .OrderBy(g => g.sort_order)
                .ThenBy(g => g.name)
                .Select(g => new GradeResponseDto
                {
                    id = g.id,
                    name = g.name,
                    level = g.level
                }).ToListAsync();
            return Response.Success(list);
        }
    }
}
