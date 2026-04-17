using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace NguyenDucHuy_2123110217_ASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            var path = Path.Combine(_env.WebRootPath ?? "wwwroot", "index.html");
            return PhysicalFile(path, "text/html");
        }

        public IActionResult Categories()
        {
            var path = Path.Combine(_env.WebRootPath ?? "wwwroot", "categories.html");
            return PhysicalFile(path, "text/html");
        }
    }
}
