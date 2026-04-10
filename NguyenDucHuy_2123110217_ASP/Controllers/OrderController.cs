using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenDucHuy_2123110217_ASP.Data;
using NguyenDucHuy_2123110217_ASP.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NguyenDucHuy_2123110217_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // ===== DTO =====
        public class OrderDto
        {
            public int OrderId { get; set; }
            public string CustomerName { get; set; } = null!;
            public string UserName { get; set; } = null!;
            public decimal TotalAmount { get; set; }
            public decimal FinalAmount { get; set; }
            public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        }

        public class OrderItemDto
        {
            public int OrderItemId { get; set; }
            public string ProductName { get; set; } = null!;
            public string VariantSKU { get; set; } = null!;
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer.Name,
                    UserName = o.User.Name,
                    TotalAmount = o.TotalAmount,
                    FinalAmount = o.FinalAmount,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductName = oi.ProductVariant.Product.Name,
                        VariantSKU = oi.ProductVariant.SKU,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                }).ToListAsync();
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Where(o => o.OrderId == id)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer.Name,
                    UserName = o.User.Name,
                    TotalAmount = o.TotalAmount,
                    FinalAmount = o.FinalAmount,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductName = oi.ProductVariant.Product.Name,
                        VariantSKU = oi.ProductVariant.SKU,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            return order;
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (order.OrderItems == null || !order.OrderItems.Any())
                return BadRequest("Order must have at least one item.");

            // Tính TotalAmount
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
            order.FinalAmount = order.TotalAmount; // Chưa áp dụng discount, coupon...

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        // PUT: api/order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderId)
                return BadRequest();

            if (order.OrderItems == null || !order.OrderItems.Any())
                return BadRequest("Order must have at least one item.");

            // Cập nhật tổng tiền
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
            order.FinalAmount = order.TotalAmount;

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            if (order.Payments.Any())
                return BadRequest("Cannot delete order that has payments.");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}