using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenDucHuy_2123110217_ASP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NguyenDucHuy_2123110217_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/profile/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new {
                    userId = u.UserId,
                    name = u.Name,
                    username = u.Username,
                    email = u.Email
                }).FirstOrDefaultAsync();

            if (user == null) return NotFound();

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Select(o => new {
                    orderId = o.OrderId,
                    total = o.TotalAmount,
                    final = o.FinalAmount,
                    status = o.Status,
                    items = o.OrderItems.Select(oi => new {
                        name = oi.ProductVariant != null && oi.ProductVariant.Product != null ? oi.ProductVariant.Product.Name : "",
                        sku = oi.ProductVariant != null ? oi.ProductVariant.SKU : "",
                        qty = oi.Quantity,
                        price = oi.Price
                    })
                }).ToListAsync();

            return Ok(new { user, orders });
        }
    }
}
