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
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // ===== DTO =====
        public class CustomerDto
        {
            public int CustomerId { get; set; }
            public string Name { get; set; } = null!;
            public string Phone { get; set; } = null!;
            public IEnumerable<int> OrderIds { get; set; } = new List<int>();
        }

        // GET: api/customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            return await _context.Customers
                .Include(c => c.Orders)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    Phone = c.Phone,
                    OrderIds = c.Orders.Select(o => o.OrderId)
                }).ToListAsync();
        }

        // GET: api/customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    Phone = c.Phone,
                    OrderIds = c.Orders.Select(o => o.OrderId)
                }).FirstOrDefaultAsync();

            if (customer == null)
                return NotFound();

            return customer;
        }

        // POST: api/customer
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            // Validate phone trùng
            if (_context.Customers.Any(c => c.Phone == customer.Phone))
                return BadRequest("Phone number already exists.");

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
        }

        // PUT: api/customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
                return BadRequest();

            // Validate phone trùng (không tính chính nó)
            if (_context.Customers.Any(c => c.Phone == customer.Phone && c.CustomerId != id))
                return BadRequest("Phone number already exists.");

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
                return NotFound();

            // Không xóa nếu khách hàng đã có đơn hàng
            if (customer.Orders.Any())
                return BadRequest("Cannot delete customer with existing orders.");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(c => c.CustomerId == id);
        }
    }
}