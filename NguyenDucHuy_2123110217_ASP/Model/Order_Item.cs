using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public class Order_Item
    {
        [Key]
        public int OrderItemId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("ProductVariant")]
        public int VariantId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public Product_Variant ProductVariant { get; set; } = null!;
    }
}