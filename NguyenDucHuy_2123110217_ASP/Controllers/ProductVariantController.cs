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
    public class ProductVariantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductVariantController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/productvariant
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product_Variant>>> GetVariants()
        {
            return await _context.ProductVariants
                                 .Include(v => v.Product)
                                 .Include(v => v.Inventory)
                                 .Include(v => v.OrderItems)
                                 .ToListAsync();
        }

        // GET: api/productvariant/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product_Variant>> GetVariant(int id)
        {
            var variant = await _context.ProductVariants
                                        .Include(v => v.Product)
                                        .Include(v => v.Inventory)
                                        .Include(v => v.OrderItems)
                                        .FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null)
                return NotFound();

            return variant;
        }

        // POST: api/productvariant
        [HttpPost]
        public async Task<ActionResult<Product_Variant>> PostVariant(Product_Variant variant)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(variant.SKU))
                return BadRequest("SKU is required.");

            if (!_context.Products.Any(p => p.ProductId == variant.ProductId))
                return BadRequest("Invalid ProductId.");

            if (variant.Price <= 0 || variant.CostPrice < 0)
                return BadRequest("Price and CostPrice must be valid.");

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVariant), new { id = variant.VariantId }, variant);
        }

        // PUT: api/productvariant/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVariant(int id, Product_Variant variant)
        {
            if (id != variant.VariantId)
                return BadRequest();

            // Validate
            if (string.IsNullOrWhiteSpace(variant.SKU))
                return BadRequest("SKU is required.");

            if (!_context.Products.Any(p => p.ProductId == variant.ProductId))
                return BadRequest("Invalid ProductId.");

            if (variant.Price <= 0 || variant.CostPrice < 0)
                return BadRequest("Price and CostPrice must be valid.");

            _context.Entry(variant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VariantExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/productvariant/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var variant = await _context.ProductVariants
                                        .Include(v => v.OrderItems)
                                        .FirstOrDefaultAsync(v => v.VariantId == id);
            if (variant == null)
                return NotFound();

            if (variant.OrderItems.Any())
                return BadRequest("Cannot delete variant linked to orders.");

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VariantExists(int id)
        {
            return _context.ProductVariants.Any(e => e.VariantId == id);
        }
    }
}