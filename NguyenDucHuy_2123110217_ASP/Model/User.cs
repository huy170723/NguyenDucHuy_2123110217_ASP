using System.ComponentModel.DataAnnotations;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // 🔹 Thêm Name

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}