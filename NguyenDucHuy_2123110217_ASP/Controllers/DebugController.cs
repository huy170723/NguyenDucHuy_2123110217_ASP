using Microsoft.AspNetCore.Mvc;
using NguyenDucHuy_2123110217_ASP.Data;
using System.Threading.Tasks;

namespace NguyenDucHuy_2123110217_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DebugController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            // Return simple counts to verify seeding and DB connectivity
            var products = await System.Threading.Tasks.Task.FromResult(_context.Products.Count());
            var categories = await System.Threading.Tasks.Task.FromResult(_context.Categories.Count());
            var variants = await System.Threading.Tasks.Task.FromResult(_context.ProductVariants.Count());

            return Ok(new { products, categories, variants });
        }
    }
}
