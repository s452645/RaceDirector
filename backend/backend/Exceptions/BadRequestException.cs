namespace backend.Exceptions
{
    public class BadRequestException : CustomHttpException
    {
        public BadRequestException(string message)
            : base(message, StatusCodes.Status400BadRequest) { }
    }
}
