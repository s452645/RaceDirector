namespace backend.Exceptions
{
    public class CustomHttpException : Exception
    {
        public int StatusCode { get; }

        public CustomHttpException()
        {
            StatusCode = 500;
        }

        public CustomHttpException(string message)
            : base(message)
        {
            StatusCode = 500;
        }

        public CustomHttpException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public CustomHttpException(string message, Exception inner)
            : base(message, inner)
        {
            StatusCode = 500;
        }
    }
}
