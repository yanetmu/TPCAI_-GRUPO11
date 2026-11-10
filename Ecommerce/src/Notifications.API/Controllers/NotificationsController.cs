using Notifications.API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Models;
using Notifications.API.Services;

namespace Notifications.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService service;

        public NotificationsController(NotificationService service)
        {
            this.service = service;
        }


        /// <summary>
        /// Envía una notificación a un usuario.
        /// </summary>
        /// <param name="request">Datos de la notificación.</param>
        /// <returns>La notificación creada.</returns>
        /// <response code="201">Notificación enviada correctamente.</response>
        /// <response code="400">Datos inválidos (NTF-002).</response>
        /// <response code="404">Usuario no encontrado (NTF-001).</response>
        /// <response code="500">Error interno del servidor (NTF-004).</response>
        /// 


        /// <remarks>
        /// Ejemplo de respuesta exitosa (201):
        ///
        ///     {
        ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///       "usuarioId": "11111111-1111-1111-1111-111111111111",
        ///       "mensaje": "Tu pedido fue confirmado.",
        ///       "tipo": "Email",
        ///       "estado": "Enviada",
        ///       "fechaEnvio": "2026-11-10T03:00:00Z"
        ///     }
        ///
        /// Ejemplo de error (400 - NTF-002):
        ///
        ///     {
        ///       "type": "about:blank",
        ///       "title": "Error",
        ///       "status": 400,
        ///       "detail": "Los datos de la notificación son inválidos.",
        ///       "instance": "/api/notifications/send",
        ///       "errorCode": "NTF-002",
        ///       "errorMessage": "Los datos de la notificación son inválidos."
        ///     }
        ///     
        /// 
        /// Ejemplo de error (404 - NTF-001):
        ///
        ///     {
        ///       "type": "about:blank",
        ///       "title": "Error",
        ///       "status": 404,
        ///       "detail": "El usuario no existe.",
        ///       "instance": "/api/notifications/send",
        ///       "errorCode": "NTF-001",
        ///       "errorMessage": "El usuario no existe."
        ///     }
        ///
        /// Ejemplo de error (500 - NTF-004):
        ///
        ///     {
        ///       "type": "about:blank",
        ///       "title": "Error",
        ///       "status": 500,
        ///       "detail": "Error interno del servidor.",
        ///       "instance": "/api/notifications/send",
        ///       "errorCode": "NTF-004",
        ///       "errorMessage": "Error interno del servidor."
        ///     }
        ///
        /// </remarks>
        /// 



        


        [HttpPost("send")]
        [ProducesResponseType(typeof(Notification), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult Enviar(SendNotificationRequest request)
        {
            Notification notificacion = service.Enviar(request);

            return StatusCode(201, notificacion);
        }


        /// <summary>
        /// Obtiene las notificaciones de un usuario.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        /// <returns>Lista de notificaciones del usuario.</returns>
        /// <response code="200">Notificaciones obtenidas correctamente.</response>
        /// <response code="404">No se encontraron notificaciones (NTF-003).</response>
        /// <response code="500">Error interno del servidor (NTF-004).</response>
        /// 


        /// <remarks>
        /// Ejemplo de respuesta exitosa (200):
        ///
        ///     [
        ///       {
        ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "usuarioId": "11111111-1111-1111-1111-111111111111",
        ///         "mensaje": "Tu pedido fue confirmado.",
        ///         "tipo": "Email",
        ///         "estado": "Enviada",
        ///         "fechaEnvio": "2026-11-10T03:00:00Z"
        ///       }
        ///     ]
        ///
        /// Ejemplo de error (404 - NTF-003):
        ///
        ///     {
        ///       "type": "about:blank",
        ///       "title": "Error",
        ///       "status": 404,
        ///       "detail": "No se encontraron notificaciones para el usuario.",
        ///       "instance": "/api/notifications/11111111-1111-1111-1111-111111111111",
        ///       "errorCode": "NTF-003",
        ///       "errorMessage": "No se encontraron notificaciones para el usuario."
        ///     }
        ///
        /// Ejemplo de error (500 - NTF-004):
        ///
        ///     {
        ///       "type": "about:blank",
        ///       "title": "Error",
        ///       "status": 500,
        ///       "detail": "Error interno del servidor.",
        ///       "instance": "/api/notifications/11111111-1111-1111-1111-111111111111",
        ///       "errorCode": "NTF-004",
        ///       "errorMessage": "Error interno del servidor."
        ///     }
        ///
        /// </remarks>

        [HttpGet("{usuarioId:guid}")]
        [ProducesResponseType(typeof(List<Notification>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult ObtenerPorUsuario(Guid usuarioId)
        {
            List<Notification> notificaciones =
                service.ObtenerPorUsuario(usuarioId);

            if (notificaciones.Count == 0)
            {
                if (notificaciones.Count == 0)
                {
                    throw new NotFoundException(
                        "NTF-003",
                        "No se encontraron notificaciones para el usuario.");
                }
            }

            return Ok(notificaciones);
        }
    }
}

