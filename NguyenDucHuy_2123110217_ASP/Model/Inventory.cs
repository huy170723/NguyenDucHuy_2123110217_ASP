using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [ForeignKey("Product_Variant")]
        public int VariantId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation
        public Product_Variant ProductVariant { get; set; }

        public ICollection<Stock_Movement>? StockMovements { get; set; }
    }
}