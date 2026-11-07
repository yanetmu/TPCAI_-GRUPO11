namespace Users.API.Exceptions
{
    public class UserException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }

        public UserException(string errorCode, string message, int statusCode)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}
