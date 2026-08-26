using ExamSystem.Constraints;
using ExamSystem.DAO.AdminUser;
using ExamSystem.DAO.Utilities;
using ExamSystem.DTOs.AdminUser;
using ExamSystem.DTOs.Common;
using ExamSystem.Entity;
using ExamSystem.Exceptions;
using ExamSystem.Utilities;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Services.AdminUser
{
    public interface IAdminUserService
    {
        Task<(List<AdminUserResponseDto>, int)> GetUsersAsync(AdminUserFilterDto filter);
        Task<Response> GetUserAsync(long id);
        Task<Response> CreateUserAsync(AdminUserRequestDto dto);
        Task<Response> UpdateUserAsync(long id, AdminUserRequestDto dto);
        Task<Response> DeleteUserAsync(long id);
        Task<Response> GetAllForDropdownAsync();
    }

    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserDao _dao;
        private readonly LogUtility _logUtility;

        public AdminUserService(IAdminUserDao dao, IHttpContextAccessor httpContextAccessor)
        {
            _dao = dao;
            _logUtility = LogUtility.CreateLogUtility(httpContextAccessor);
        }

        public async Task<(List<AdminUserResponseDto>, int)> GetUsersAsync(AdminUserFilterDto filter)
        {
            var query = _dao.GetAll();

            if (!string.IsNullOrWhiteSpace(filter.search))
            {
                var s = filter.search.Trim().ToLower();
                query = query.Where(u => u.username.ToLower().Contains(s)
                                      || u.full_name.ToLower().Contains(s)
                                      || (u.email != null && u.email.ToLower().Contains(s)));
            }
            if (filter.role.HasValue)
            {
                query = query.Where(u => u.role == filter.role.Value);
            }
            if (filter.is_active.HasValue)
            {
                query = query.Where(u => u.is_active == filter.is_active.Value);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(u => u.id)
                .Skip((filter.page_number - 1) * filter.page_size)
                .Take(filter.page_size)
                .ToListAsync();

            var dtos = items.Select(u => new AdminUserResponseDto
            {
                id = u.id,
                username = u.username,
                email = u.email,
                full_name = u.full_name,
                phone = u.phone,
                role = u.role,
                role_name = u.role.GetRoleName(),
                is_active = u.is_active,
                profile_image = u.profile_image,
                created_datetime = u.created_datetime,
                updated_datetime = u.updated_datetime
            }).ToList();

            return (dtos, total);
        }

        public async Task<Response> GetUserAsync(long id)
        {
            var u = await _dao.GetById(id);
            if (u == null)
            {
                return Response.Error(new Error
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = $"Admin user with Id {id} doesn't exist."
                });
            }
            return Response.Success(new AdminUserResponseDto
            {
                id = u.id,
                username = u.username,
                email = u.email,
                full_name = u.full_name,
                phone = u.phone,
                role = u.role,
                role_name = u.role.GetRoleName(),
                is_active = u.is_active,
                profile_image = u.profile_image,
                created_datetime = u.created_datetime,
                updated_datetime = u.updated_datetime
            });
        }

        public async Task<Response> CreateUserAsync(AdminUserRequestDto dto)
        {
            if (await _dao.UsernameExists(dto.username))
            {
                return Response.Error(new Error
                {
                    Status = 409,
                    Title = "Duplicate Username",
                    Detail = "Username is already taken."
                });
            }
            if (!string.IsNullOrWhiteSpace(dto.email) && await _dao.EmailExists(dto.email))
            {
                return Response.Error(new Error
                {
                    Status = 409,
                    Title = "Duplicate Email",
                    Detail = "Email is already registered."
                });
            }

            var entity = new m_admin_user
            {
                username = dto.username,
                email = dto.email,
                full_name = dto.full_name,
                phone = dto.phone,
                password_hash = Encryption.HashPassword(dto.password ?? "Exam@123"),
                role = dto.role == 0 ? UserRoles.Admin : dto.role,
                is_active = dto.is_active,
                is_deleted = false,
                created_datetime = DateTime.Now,
                updated_datetime = DateTime.Now,
                created_user_id = AuthUser.Id,
                updated_user_id = AuthUser.Id
            };
            await _dao.Add(entity);

            return Response.Success(new { id = entity.id, username = entity.username });
        }

        public async Task<Response> UpdateUserAsync(long id, AdminUserRequestDto dto)
        {
            var entity = await _dao.GetById(id);
            if (entity == null) throw new NotFoundException("User not found.");

            if (await _dao.UsernameExists(dto.username, id))
            {
                return Response.Error(new Error { Status = 409, Title = "Duplicate Username", Detail = "Username is already taken." });
            }
            if (!string.IsNullOrWhiteSpace(dto.email) && await _dao.EmailExists(dto.email, id))
            {
                return Response.Error(new Error { Status = 409, Title = "Duplicate Email", Detail = "Email is already registered." });
            }

            entity.username = dto.username;
            entity.email = dto.email;
            entity.full_name = dto.full_name;
            entity.phone = dto.phone;
            entity.role = dto.role == 0 ? entity.role : dto.role;
            entity.is_active = dto.is_active;
            entity.updated_datetime = DateTime.Now;
            entity.updated_user_id = AuthUser.Id;

            if (!string.IsNullOrWhiteSpace(dto.password))
            {
                entity.password_hash = Encryption.HashPassword(dto.password);
            }

            await _dao.Update(entity);
            return Response.Success(new { id = entity.id });
        }

        public async Task<Response> DeleteUserAsync(long id)
        {
            var entity = await _dao.GetById(id);
            if (entity == null) throw new NotFoundException("User not found.");

            if (entity.id == AuthUser.Id)
            {
                return Response.Error(new Error { Status = 400, Title = "Invalid Action", Detail = "You cannot delete your own account." });
            }

            await _dao.Delete(entity);
            return Response.Success("User deleted successfully.");
        }

        public async Task<Response> GetAllForDropdownAsync()
        {
            var list = await _dao.GetAll()
                .Where(u => u.is_active)
                .OrderBy(u => u.full_name)
                .Select(u => new AdminUserResponseDto
                {
                    id = u.id,
                    username = u.username,
                    full_name = u.full_name,
                    role = u.role,
                    role_name = u.role.GetRoleName()
                }).ToListAsync();
            return Response.Success(list);
        }
    }
}
