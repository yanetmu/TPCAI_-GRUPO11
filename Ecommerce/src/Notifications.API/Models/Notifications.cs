
namespace Notifications.API.Models
{
    /// <summary>
    /// Representa una notificación enviada a un usuario.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Identificador único de la notificación.
        /// </summary>
        /// /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del usuario destinatario.
        /// </summary>
        /// 
        public Guid UsuarioId { get; set; }

        /// <summary>
        /// Contenido del mensaje (máximo 500 caracteres).
        /// </summary>
        /// /// <example>Tu pedido fue confirmado.</example>
        public string Mensaje { get; set; } = "";

        /// <summary>
        /// Tipo de notificación: Email, Push o SMS.
        /// </summary>
        /// /// <example>Email</example>
        public string Tipo { get; set; } = "";

        /// <summary>
        /// Estado de la notificación: Pendiente, Enviada o Fallida.
        /// </summary>
        /// /// <example>Enviada</example>
        public string Estado { get; set; } = "";

        /// <summary>
        /// Fecha y hora de envío de la notificación.
        /// </summary>
        /// /// <example>2026-11-10T03:00:00Z</example>
        public DateTime FechaEnvio { get; set; }
    }
}








