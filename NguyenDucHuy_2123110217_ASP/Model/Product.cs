using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }

        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Category Category { get; set; }

        // URL ảnh hiển thị sản phẩm (tuỳ chọn)
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        // ⚡ Quan trọng: đặt trùng với DbContext
        public ICollection<Product_Variant>? ProductVariants { get; set; }
    }
}