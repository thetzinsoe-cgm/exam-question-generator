using ExamSystem.Attributes;
using ExamSystem.Constraints;
using ExamSystem.DTOs.AdminUser;
using ExamSystem.Exceptions;
using ExamSystem.Helpers;
using ExamSystem.Services;
using ExamSystem.Services.AdminUser;
using ExamSystem.Utilities;
using ExamSystem.ViewModels.AdminUser;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers.Admin
{
    [Route("admin")]
    [AuthorizeUser(UserRoles.SuperAdmin, UserRoles.Admin)]
    public class AdminUserController : BaseController
    {
        private readonly IAdminUserService _service;
        private readonly SessionService _session;

        public AdminUserController(IAdminUserService service, SessionService sessionService)
        {
            _service = service;
            _session = sessionService;
        }

        [HttpGet("admin-user/index")]
        public async Task<IActionResult> Index([FromQuery] AdminUserFilterDto filter)
        {
            filter ??= new AdminUserFilterDto();
            var (list, total) = await _service.GetUsersAsync(filter);
            var paginated = PaginationHelper.Paginated(list, filter.page_number, filter.page_size, total, HttpContext.Request);

            var vm = new AdminUserIndexViewModel
            {
                Filter = filter,
                PageSizeOptions = DropDownHelper.GetPageSizeOptions(filter.page_size),
                Response = paginated
            };
            return View("~/Views/AdminUser/Index.cshtml", vm);
        }

        [HttpGet("admin-user/create")]
        public IActionResult Create()
        {
            ViewBag.FormTitle = "Create Admin User";
            return View("~/Views/AdminUser/Create.cshtml");
        }

        [HttpPost("admin-user/create")]
        public async Task<IActionResult> Create(AdminUserRequestDto request)
        {
            ViewBag.FormTitle = "Create Admin User";
            if (!ModelState.IsValid) return View("~/Views/AdminUser/Create.cshtml", request);

            var resp = await _service.CreateUserAsync(request);
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
                return View("~/Views/AdminUser/Create.cshtml", request);
            }
            SuccessMessage("Admin user created successfully.");
            return RedirectToAction("Create");
        }

        [HttpGet("admin-user/edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            ViewBag.FormTitle = "Edit Admin User";
            var resp = await _service.GetUserAsync(id);
            if (resp.Errors != null) return NotFound();
            var dto = resp.Data as AdminUserResponseDto;
            return View("~/Views/AdminUser/Edit.cshtml", dto);
        }

        [HttpPost("admin-user/edit/{id}")]
        public async Task<IActionResult> Edit(long id, AdminUserResponseDto input, string listingPageUrl)
        {
            ViewBag.FormTitle = "Edit Admin User";
            var request = new AdminUserRequestDto
            {
                username = input.username,
                email = input.email,
                full_name = input.full_name,
                phone = input.phone,
                role = input.role,
                is_active = input.is_active
            };
            var resp = await _service.UpdateUserAsync(id, request);
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
                return View("~/Views/AdminUser/Edit.cshtml", input);
            }
            SuccessMessage("Admin user updated successfully.");
            return !string.IsNullOrWhiteSpace(listingPageUrl)
                ? Redirect(listingPageUrl)
                : RedirectToAction("Index");
        }

        [HttpGet("admin-user/delete/{id}")]
        public async Task<IActionResult> Delete(long id, int page_number = 1)
        {
            try
            {
                var resp = await _service.DeleteUserAsync(id);
                if (resp.IsSuccess) SuccessMessage("Admin user deleted successfully.");
                else if (resp.Errors != null) ErrorMessage(resp.Errors.Detail);
            }
            catch (NotFoundException nfex)
            {
                ErrorMessage(nfex.Message);
            }
            return RedirectToAction("Index", new { page_number });
        }
    }
}
