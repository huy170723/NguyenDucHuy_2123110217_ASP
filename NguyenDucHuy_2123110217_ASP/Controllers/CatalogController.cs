using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenDucHuy_2123110217_ASP.Data;
using NguyenDucHuy_2123110217_ASP.Model;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NguyenDucHuy_2123110217_ASP.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext _context;
        public CatalogController(AppDbContext context)
        {
            _context = context;
        }

        public class ProductListViewModel
        {
            public IEnumerable<Product> Products { get; set; } = new List<Product>();
            public IEnumerable<Category> Categories { get; set; } = new List<Category>();
            public int SelectedCategoryId { get; set; }
            public string? SearchQuery { get; set; }
            public string? Sort { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / PageSize);
        }

        // GET: /Catalog
        public async Task<IActionResult> Index(int? categoryId, string? q, string? sort, int page = 1, int pageSize = 12)
        {
            // load categories for sidebar/dropdown
            var categories = await _context.Categories
                                           .Include(c => c.Products)
                                           .ToListAsync();

            var query = _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.ProductVariants)
                                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p => EF.Functions.Like(p.Name, $"%{q}%"));
            }

            // sorting
            sort = sort ?? "newest";
            switch (sort)
            {
                case "name": query = query.OrderBy(p => p.Name); break;
                case "price_asc": query = query.OrderBy(p => p.Price); break;
                case "price_desc": query = query.OrderByDescending(p => p.Price); break;
                default: query = query.OrderByDescending(p => p.CreatedAt); break;
            }

            var total = await query.CountAsync();
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var vm = new ProductListViewModel
            {
                Products = products,
                Categories = categories,
                SelectedCategoryId = categoryId ?? 0,
                SearchQuery = q,
                Sort = sort,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };

            return View(vm);
        }
    }
}
