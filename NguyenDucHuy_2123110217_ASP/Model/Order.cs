using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public decimal FinalAmount { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = null!;

        // Navigation
        public Customer? Customer { get; set; }
        public User? User { get; set; }

        public ICollection<Order_Item>? OrderItems { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}