namespace backend.Exceptions
{
    public class NotFoundException : CustomHttpException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound) { }
    }
}
