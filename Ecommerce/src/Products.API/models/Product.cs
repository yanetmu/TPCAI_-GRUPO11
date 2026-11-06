using System.ComponentModel.DataAnnotations;

namespace Products.API.models
{
    /// <summary>
    /// Representa un producto del catálogo.
    /// </summary>
    public class Product
    {

        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335",ParseLimitsInInvariantCulture = true)]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public string Categoria { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
