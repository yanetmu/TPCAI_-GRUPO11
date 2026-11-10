
namespace Notifications.API.Exceptions
{
    public class NotFoundException : Exception
    {
        public string ErrorCode { get; set; }

        public NotFoundException(string errorCode, string mensaje)
            : base(mensaje)
        {
            ErrorCode = errorCode;
        }
    }
}

