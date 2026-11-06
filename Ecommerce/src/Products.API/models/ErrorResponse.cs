
namespace Products.API.models
{
    /// <summary>
    /// Representa una respuesta de error de Products.API.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>Tipo de error HTTP.</summary>
        
        public string Type { get; set; } = string.Empty;

        /// <summary>Título del error.</summary>
        
        public string Title { get; set; } = string.Empty;

        /// <summary>Código de estado HTTP.</summary>
        
        public int Status { get; set; }

        /// <summary>Detalle del error.</summary>
        
        public string Detail { get; set; } = string.Empty;

        /// <summary>Ruta donde ocurrió el error.</summary>
        
        public string Instance { get; set; } = string.Empty;

        /// <summary>Código de error de Products.API.</summary>
        
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>Mensaje del error.</summary>
        
        public string ErrorMessage { get; set; } = string.Empty;
    }
}

