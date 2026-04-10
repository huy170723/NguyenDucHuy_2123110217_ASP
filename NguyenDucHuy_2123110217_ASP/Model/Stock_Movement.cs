using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public class Stock_Movement
    {
        [Key]
        public int MovementId { get; set; }

        [ForeignKey("Inventory")]
        public int InventoryId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = null!;  // IN / OUT / ADJUST

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Inventory Inventory { get; set; } = null!;
    }
}