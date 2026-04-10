using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public enum PaymentMethod
    {
        Cash,
        Bank,
        EWallet
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // Navigation
        public Order Order { get; set; } = null!;
    }
}