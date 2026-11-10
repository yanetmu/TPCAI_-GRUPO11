namespace Orders.API.Exceptions
{
    public class OrderException : Exception
    {
        public string ErrorCode { get; set; }
        public int StatusCode { get; set; }

        public OrderException(string errorCode, string mensaje, int statusCode)
            : base(mensaje)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}
