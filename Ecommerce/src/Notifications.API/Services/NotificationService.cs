
using Notifications.API.Data;
using Notifications.API.DTOs;
using Notifications.API.Models;
using Notifications.API.Exceptions;

namespace Notifications.API.Services
{
    public class NotificationService
    {
        private readonly NotificationRepository repository;

        public NotificationService(NotificationRepository repository)
        {
            this.repository = repository;
        }

        public Notification Enviar(SendNotificationRequest request)
        {
            if (request.UsuarioId == Guid.Empty ||
               string.IsNullOrWhiteSpace(request.Mensaje) || request.Mensaje.Length > 500 ||
                string.IsNullOrWhiteSpace(request.Tipo))
            {
                throw new ValidationException("Los datos de la notificación son inválidos.");

            }

            if (request.Tipo != "Email" && request.Tipo != "SMS" && request.Tipo != "Push")
            {
                throw new ValidationException("Los datos de la notificación son inválidos.");
            }


            Notification notificacion = new Notification();

            notificacion.Id = Guid.NewGuid();
            notificacion.UsuarioId = request.UsuarioId;
            notificacion.Mensaje = request.Mensaje;
            notificacion.Tipo = request.Tipo;
            notificacion.Estado = "Enviada";
            notificacion.FechaEnvio = DateTime.UtcNow;

            repository.Agregar(notificacion);

            return notificacion;
        }

        public List<Notification> ObtenerPorUsuario(Guid usuarioId)
        {
            return repository.ObtenerPorUsuario(usuarioId);
        }
    }
}


