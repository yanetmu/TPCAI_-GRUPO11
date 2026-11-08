using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs
{
    /// <summary>
    /// Datos necesarios para iniciar sesion.
    /// </summary>
    public class LoginRequest
    {
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
