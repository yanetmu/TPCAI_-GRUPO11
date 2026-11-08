namespace Users.API.DTOs
{
    /// <summary>
    /// Datos que se devuelven al registrar un usuario.
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>
        /// Email del usuario.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Fecha en que se registro el usuario.
        /// </summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Indica si el usuario esta activo.
        /// </summary>
        public bool Activo { get; set; }
    }
}

