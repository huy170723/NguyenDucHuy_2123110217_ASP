using Microsoft.AspNetCore.Mvc;

namespace NguyenDucHuy_2123110217_ASP.Controllers
{
    [Route("Profile")]
    public class ProfilePageController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            // return the Razor view located at Views/Profile/Index.cshtml
            return View("~/Views/Profile/Index.cshtml");
        }
    }
}
