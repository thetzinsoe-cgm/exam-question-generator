using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers.Admin
{
    [Route("errors")]
    public class ErrorsController : Controller
    {
        [Route("401")]
        public IActionResult E401() => View("~/Views/errors/401.cshtml");

        [Route("403")]
        public IActionResult E403() => View("~/Views/errors/403.cshtml");

        [Route("404")]
        public IActionResult E404() => View("~/Views/errors/404.cshtml");

        [Route("500")]
        public IActionResult E500() => View("~/Views/errors/500.cshtml");

        [Route("{code}")]
        public IActionResult ByCode(int code)
        {
            return code switch
            {
                401 => E401(),
                403 => E403(),
                404 => E404(),
                500 => E500(),
                _ => View("~/Views/errors/404.cshtml")
            };
        }
    }
}
