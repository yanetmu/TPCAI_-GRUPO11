
namespace Notifications.API.DTOs
{
    /// <summary>
    /// Representa una respuesta de error de la API.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Tipo de error.
        /// </summary>
        /// <example>about:blank</example>
        public string Type { get; set; } = "";

        /// <summary>
        /// Título del error.
        /// </summary>
        /// <example>Error</example>
        public string Title { get; set; } = "";

        /// <summary>
        /// Código de estado HTTP.
        /// </summary>
        /// <example>400</example>
        public int Status { get; set; }

        /// <summary>
        /// Descripción del error.
        /// </summary>
        /// <example>Los datos de la notificación son inválidos.</example>
        public string Detail { get; set; } = "";

        /// <summary>
        /// Ruta donde ocurrió el error.
        /// </summary>
        /// <example>/api/notifications/send</example>
        public string Instance { get; set; } = "";

        /// <summary>
        /// Código identificador del error.
        /// </summary>
        /// <example>NTF-002</example>
        public string ErrorCode { get; set; } = "";

        /// <summary>
        /// Mensaje que describe el error.
        /// </summary>
        /// <example>Los datos de la notificación son inválidos.</example>
        public string ErrorMessage { get; set; } = "";

        /// <summary>
        /// Identificador de correlación de la solicitud.
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public string CorrelationId { get; set; } = "";
    }
}

