

using Notifications.API.Models;

namespace Notifications.API.Data
{
    public class NotificationRepository
    {
        private readonly List<Notification> notificaciones = new List<Notification>();

        public void Agregar(Notification notificacion)
        {
            notificaciones.Add(notificacion);
        }

        public List<Notification> ObtenerPorUsuario(Guid usuarioId)
        {
            List<Notification> resultado = new List<Notification>();

            foreach (Notification notificacion in notificaciones)
            {
                if (notificacion.UsuarioId == usuarioId)
                {
                    resultado.Add(notificacion);
                }
            }

            return resultado;
        }
    }
}


