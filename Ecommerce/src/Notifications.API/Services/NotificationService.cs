
using Notifications.API.Data;
using Notifications.API.DTOs;
using Notifications.API.Models;
using Notifications.API.Exceptions;

namespace Notifications.API.Services
{
    public class NotificationService
    {
        private readonly NotificationRepository repository;

        
        private readonly HttpClient httpClient;

        public NotificationService(
            NotificationRepository repository,
            IHttpClientFactory httpClientFactory)
        {
            this.repository = repository;
            this.httpClient = httpClientFactory.CreateClient();
        }


        private async Task<bool> ExisteUsuario(Guid usuarioId)
        {
            HttpResponseMessage respuesta = await httpClient.GetAsync(
                $"https://localhost:7191/api/users/{usuarioId}");

            if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            respuesta.EnsureSuccessStatusCode();

            return true;
        }


        public async Task<Notification> Enviar(SendNotificationRequest request)
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

            bool existe = await ExisteUsuario(request.UsuarioId);

            if (!existe)
            {
                throw new NotFoundException("NTF-001", "El usuario no existe.");
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


