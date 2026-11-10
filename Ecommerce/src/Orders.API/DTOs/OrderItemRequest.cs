using System.ComponentModel.DataAnnotations;

namespace Orders.API.DTOs
{
    public class OrderItemRequest
    {
        [Required]
        public Guid ProductoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }
}