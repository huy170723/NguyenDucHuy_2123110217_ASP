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
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            // Include Category và ProductVariants
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.ProductVariants)
                                 .ToListAsync();
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .Include(p => p.ProductVariants)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            return product;
        }

        // GET: api/product/category/5
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            // Kiểm tra category tồn tại
            if (!_context.Categories.Any(c => c.CategoryId == categoryId))
                return NotFound($"Category with id {categoryId} not found.");

            var products = await _context.Products
                                         .Where(p => p.CategoryId == categoryId)
                                         .Include(p => p.Category)
                                         .Include(p => p.ProductVariants)
                                         .ToListAsync();

            return products;
        }

        // GET: api/product/search?q=iphone
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts([FromQuery(Name = "q")] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Query parameter 'q' is required.");

            // Sử dụng EF.Functions.Like để tìm tên chứa chuỗi (case-insensitive tuỳ DB collation)
            var pattern = $"%{q}%";
            var results = await _context.Products
                                        .Where(p => EF.Functions.Like(p.Name, pattern))
                                        .Include(p => p.Category)
                                        .Include(p => p.ProductVariants)
                                        .ToListAsync();

            return results;
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(product.Name))
                return BadRequest("Product name is required.");

            if (!_context.Categories.Any(c => c.CategoryId == product.CategoryId))
                return BadRequest("Invalid CategoryId.");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // PUT: api/product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
                return BadRequest();

            // Validate
            if (string.IsNullOrWhiteSpace(product.Name))
                return BadRequest("Product name is required.");

            if (!_context.Categories.Any(c => c.CategoryId == product.CategoryId))
                return BadRequest("Invalid CategoryId.");

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.ProductVariants)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                return NotFound();

            // Không xóa nếu còn variant liên quan
            if (product.ProductVariants?.Any() ?? false)
                return BadRequest("Cannot delete product with variants.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}