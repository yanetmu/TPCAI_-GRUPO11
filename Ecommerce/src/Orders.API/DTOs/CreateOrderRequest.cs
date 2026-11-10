using System.ComponentModel.DataAnnotations;

namespace Orders.API.DTOs
{
    public class CreateOrderRequest
    {
        [Required]
        public Guid UsuarioId { get; set; }

        [Required]
        [MinLength(1)]
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }
}