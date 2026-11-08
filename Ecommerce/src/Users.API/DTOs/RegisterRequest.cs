using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs
{
    /// <summary>
    /// Datos necesarios para registrar un usuario.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        [Required]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        [Required]
        public string Apellido { get; set; } = string.Empty;

        /// <summary>
        /// Email del usuario.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
