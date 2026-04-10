using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public class Product_Variant
    {
        [Key]
        public int VariantId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Size { get; set; }

        [Required]
        [MaxLength(50)]
        public string Color { get; set; }

        [Required]
        [MaxLength(100)]
        public string SKU { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }

        // Navigation
        public Product Product { get; set; }

        public ICollection<Order_Item>? OrderItems { get; set; }

        public Inventory? Inventory { get; set; }
    }
}