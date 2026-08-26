using ExamSystem.DTOs;
using ExamSystem.DTOs.AdminUser;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExamSystem.ViewModels
{
    public class PaginationViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class DropdownViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}

namespace ExamSystem.ViewModels.Auth
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public short Role { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
        public string ResetToken { get; set; }
    }
}

namespace ExamSystem.ViewModels.Common
{
    public class IndexViewModel<T>
    {
        public Paginated<T> Response { get; set; }
        public List<SelectListItem> PageSizeOptions { get; set; }
    }
}

namespace ExamSystem.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int TotalGrade { get; set; }
        public int TotalSubject { get; set; }
        public int TotalQuestionCount { get; set; }
        public int TotalExam { get; set; }
        public int TotalActiveAdmin { get; set; }
        public int TotalMarkingRules { get; set; }
    }
}

namespace ExamSystem.ViewModels.AdminUser
{
    public class AdminUserIndexViewModel
    {
        public AdminUserFilterDto Filter { get; set; }
        public Paginated<AdminUserResponseDto> Response { get; set; }
        public List<SelectListItem> PageSizeOptions { get; set; }
    }
}
