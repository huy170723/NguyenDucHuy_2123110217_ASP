using System.Collections.Generic;

namespace NguyenDucHuy_2123110217_ASP.Controllers
{
    public class OrderCreateDto
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string? Status { get; set; }
        public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
    }

    public class OrderItemCreateDto
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
