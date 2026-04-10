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
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        // ===== DTO =====
        public class InventoryDto
        {
            public int InventoryId { get; set; }
            public int VariantId { get; set; }
            public int Quantity { get; set; }
            public string ProductName { get; set; } = null!;
            public string VariantSKU { get; set; } = null!;
            public string Size { get; set; } = null!;
            public string Color { get; set; } = null!;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetInventories()
        {
            return await _context.Inventories
                .Include(i => i.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .Select(i => new InventoryDto
                {
                    InventoryId = i.InventoryId,
                    VariantId = i.VariantId,
                    Quantity = i.Quantity,
                    ProductName = i.ProductVariant.Product.Name,
                    VariantSKU = i.ProductVariant.SKU,
                    Size = i.ProductVariant.Size,
                    Color = i.ProductVariant.Color
                }).ToListAsync();
        }

        // GET: api/inventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryDto>> GetInventory(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .Where(i => i.InventoryId == id)
                .Select(i => new InventoryDto
                {
                    InventoryId = i.InventoryId,
                    VariantId = i.VariantId,
                    Quantity = i.Quantity,
                    ProductName = i.ProductVariant.Product.Name,
                    VariantSKU = i.ProductVariant.SKU,
                    Size = i.ProductVariant.Size,
                    Color = i.ProductVariant.Color
                }).FirstOrDefaultAsync();

            if (inventory == null)
                return NotFound();

            return inventory;
        }

        // POST: api/inventory
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {
            if (inventory.Quantity < 0)
                return BadRequest("Quantity cannot be negative.");

            // Kiểm tra VariantId tồn tại
            if (!await _context.ProductVariants.AnyAsync(v => v.VariantId == inventory.VariantId))
                return BadRequest("VariantId does not exist.");

            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInventory), new { id = inventory.InventoryId }, inventory);
        }

        // PUT: api/inventory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventory(int id, Inventory inventory)
        {
            if (id != inventory.InventoryId)
                return BadRequest();

            if (inventory.Quantity < 0)
                return BadRequest("Quantity cannot be negative.");

            _context.Entry(inventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.StockMovements)
                .FirstOrDefaultAsync(i => i.InventoryId == id);

            if (inventory == null)
                return NotFound();

            if (inventory.StockMovements.Any())
                return BadRequest("Cannot delete inventory with stock movements.");

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.InventoryId == id);
        }
    }
}