using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NguyenDucHuy_2123110217_ASP.Model
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }   // category_id (PK)

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }      // name

        // Navigation property (1 Category có nhiều Product)
        public ICollection<Product> Products { get; set; }
    }
}