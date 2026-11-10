using System.ComponentModel.DataAnnotations;

namespace Orders.API.DTOs
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        public string Estado { get; set; } = string.Empty;
    }
}
