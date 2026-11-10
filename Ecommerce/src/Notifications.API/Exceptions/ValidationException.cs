namespace Notifications.API.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string mensaje) : base(mensaje)
        {
        }
    }
}