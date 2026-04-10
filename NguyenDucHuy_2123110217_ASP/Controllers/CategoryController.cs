using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenDucHuy_2123110217_ASP.Data;
using NguyenDucHuy_2123110217_ASP.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    // ===== DTO =====
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string Brand { get; set; } = null!;
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

    // GET: api/category
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        return await _context.Categories
            .Include(c => c.Products)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Products = c.Products.Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Brand = p.Brand
                })
            }).ToListAsync();
    }

    // GET: api/category/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .Where(c => c.CategoryId == id)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Products = c.Products.Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Brand = p.Brand
                })
            }).FirstOrDefaultAsync();

        if (category == null)
            return NotFound();

        return category;
    }

    // POST: api/category
    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(Category category)
    {
        // Validate trùng tên
        if (_context.Categories.Any(c => c.Name == category.Name))
            return BadRequest("Category name already exists.");

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
    }

    // PUT: api/category/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(int id, Category category)
    {
        if (id != category.CategoryId)
            return BadRequest();

        // Validate trùng tên (không tính chính nó)
        if (_context.Categories.Any(c => c.Name == category.Name && c.CategoryId != id))
            return BadRequest("Category name already exists.");

        _context.Entry(category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/category/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
            return NotFound();

        // Không xóa nếu còn sản phẩm
        if (category.Products.Any())
            return BadRequest("Cannot delete category with products.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.CategoryId == id);
    }
}