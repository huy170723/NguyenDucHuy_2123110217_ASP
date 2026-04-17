using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NguyenDucHuy_2123110217_ASP.Model;

namespace NguyenDucHuy_2123110217_ASP.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Ensure database created (migrations preferred)
            context.Database.EnsureCreated();

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Nam" },
                    new Category { Name = "Nữ" },
                    new Category { Name = "Phụ kiện" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Seed Products + Variants + Inventories
            if (!context.Products.Any())
            {
                var catNam = context.Categories.First(c => c.Name == "Nam");
                var catNu = context.Categories.First(c => c.Name == "Nữ");
                var catPk = context.Categories.First(c => c.Name == "Phụ kiện");

                var products = new List<Product>
                {
                    new Product { Name = "Áo Thun Basic", CategoryId = catNam.CategoryId, Brand = "Yame", Description = "Áo thun cotton mềm mại", Price = 129000m, ImageUrl = "https://via.placeholder.com/450x600?text=Aothun" },
                    new Product { Name = "Quần Jeans", CategoryId = catNam.CategoryId, Brand = "Yame", Description = "Quần jeans co giãn", Price = 299000m, ImageUrl = "https://via.placeholder.com/450x600?text=Jeans" },
                    new Product { Name = "Váy Nữ Xinh", CategoryId = catNu.CategoryId, Brand = "Yame", Description = "Váy nữ dịu dàng", Price = 349000m, ImageUrl = "https://via.placeholder.com/450x600?text=Vay" },
                    new Product { Name = "Áo Khoác", CategoryId = catNam.CategoryId, Brand = "Yame", Description = "Áo khoác giữ ấm", Price = 399000m, ImageUrl = "https://via.placeholder.com/450x600?text=Aokhoac" },
                    new Product { Name = "Túi Tote", CategoryId = catPk.CategoryId, Brand = "Yame", Description = "Túi vải tiện lợi", Price = 99000m, ImageUrl = "https://via.placeholder.com/450x600?text=Tui" },
                    new Product { Name = "Mũ Lưỡi Trai", CategoryId = catPk.CategoryId, Brand = "Yame", Description = "Mũ thời trang", Price = 79000m, ImageUrl = "https://via.placeholder.com/450x600?text=Mu" }
                };

                context.Products.AddRange(products);
                context.SaveChanges();

                var variants = new List<Product_Variant>();
                foreach (var p in context.Products.ToList())
                {
                    if (p.CategoryId == catPk.CategoryId)
                    {
                        variants.Add(new Product_Variant
                        {
                            ProductId = p.ProductId,
                            Size = "OneSize",
                            Color = "Default",
                            SKU = $"SKU-{p.ProductId}-A",
                            Price = p.Price,
                            CostPrice = p.Price * 0.6m
                        });
                    }
                    else
                    {
                        var sizes = new[] { "S", "M", "L" };
                        var colors = new[] { "Đen", "Trắng" };
                        foreach (var s in sizes)
                        foreach (var c in colors)
                        {
                            variants.Add(new Product_Variant
                            {
                                ProductId = p.ProductId,
                                Size = s,
                                Color = c,
                                SKU = $"SKU-{p.ProductId}-{s}-{c}",
                                Price = p.Price,
                                CostPrice = p.Price * 0.6m
                            });
                        }
                    }
                }

                context.ProductVariants.AddRange(variants);
                context.SaveChanges();

                var inventories = new List<Inventory>();
                foreach (var v in context.ProductVariants.ToList())
                {
                    inventories.Add(new Inventory { VariantId = v.VariantId, Quantity = 50 });
                }
                context.Inventories.AddRange(inventories);
                context.SaveChanges();
            }

            // Seed Users
            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User { Name = "Admin", Username = "admin", Password = HashPassword("admin123"), Email = "admin@yame.vn" },
                    new User { Name = "Customer1", Username = "user1", Password = HashPassword("user123"), Email = "user1@yame.vn" }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            // Seed Customers
            if (!context.Customers.Any())
            {
                var customers = new List<Customer>
                {
                    new Customer { Name = "Nguyen Van A", Phone = "0909000001" },
                    new Customer { Name = "Tran Thi B", Phone = "0909000002" }
                };
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
