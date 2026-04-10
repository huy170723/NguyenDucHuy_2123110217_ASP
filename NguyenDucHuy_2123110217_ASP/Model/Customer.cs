using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }   // thêm email

        [MaxLength(500)]
        public string? Address { get; set; } // địa chỉ

        public DateTime CreatedAt { get; set; } = DateTime.Now; // ngày tạo

        // 🔥 Quan hệ với Order
        public ICollection<Order>? Orders { get; set; }
    }
}