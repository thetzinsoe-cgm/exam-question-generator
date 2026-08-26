using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers
{
    public abstract class BaseController : Controller
    {
        public IActionResult SendErrorResponse(System.Net.HttpStatusCode statusCode)
        {
            if (statusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewData["Is404"] = true;
                return View("~/Views/errors/404.cshtml");
            }
            return RedirectToAction("Login", "Auth");
        }

        protected void SuccessMessage(string message, string title = "Info")
        {
            TempData["ShowToast"] = true;
            TempData["ToastTitle"] = title;
            TempData["ToastIcon"] = "fa-check-circle text-success";
            TempData["ToastMessage"] = message;
        }

        protected void WarningMessage(string message, string title = "Warning")
        {
            TempData["ShowToast"] = true;
            TempData["ToastTitle"] = title;
            TempData["ToastIcon"] = "fa-exclamation-triangle text-warning";
            TempData["ToastMessage"] = message;
        }

        protected void ErrorMessage(string message, string title = "Error")
        {
            TempData["ShowToast"] = true;
            TempData["ToastTitle"] = title;
            TempData["ToastIcon"] = "fa-times-circle text-danger";
            TempData["ToastMessage"] = message;
        }
    }
}
