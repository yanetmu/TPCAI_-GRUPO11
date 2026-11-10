
namespace Notifications.API.DTOs
{
    /// <summary>
    /// Datos necesarios para enviar una notificación.
    /// </summary>
    public class SendNotificationRequest
    {
        /// <summary>
        /// Identificador del usuario destinatario.
        /// </summary>
        public Guid UsuarioId { get; set; }

        /// <summary>
        /// Mensaje de la notificación. Máximo 500 caracteres.
        /// </summary>
        public string Mensaje { get; set; } = "";

        /// <summary>
        /// Tipo de notificación: Email, Push o SMS.
        /// </summary>
        public string Tipo { get; set; } = "";
    }
}


