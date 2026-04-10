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
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return await _context.Payments
                                 .Include(p => p.Order)
                                     .ThenInclude(o => o.OrderItems)
                                 .ToListAsync();
        }

        // GET: api/payment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments
                                        .Include(p => p.Order)
                                            .ThenInclude(o => o.OrderItems)
                                        .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
                return NotFound();

            return payment;
        }

        // POST: api/payment
        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            // Validate
            if (payment.Amount <= 0)
                return BadRequest("Payment amount must be greater than 0.");

            var order = await _context.Orders
                                      .Include(o => o.Payments)
                                      .FirstOrDefaultAsync(o => o.OrderId == payment.OrderId);

            if (order == null)
                return BadRequest("Order does not exist.");

            decimal totalPaid = order.Payments.Sum(p => p.Amount);
            if (totalPaid + payment.Amount > order.FinalAmount)
                return BadRequest("Payment exceeds remaining order amount.");

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, payment);
        }

        // PUT: api/payment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayment(int id, Payment payment)
        {
            if (id != payment.PaymentId)
                return BadRequest();

            if (payment.Amount <= 0)
                return BadRequest("Payment amount must be greater than 0.");

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/payment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();

            // Optional: không cho xóa payment nếu đã xác nhận / xuất báo cáo
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }
    }
}