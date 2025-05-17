namespace Services.Logging
{
    public class ExceptionDetails
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public ExceptionDetails InnerException { get; set; }

        public ExceptionDetails(Exception exception)
        {
            Message = exception.Message;
            StackTrace = exception.StackTrace;
            if (exception.InnerException != null)
            {
                InnerException = new ExceptionDetails(exception.InnerException);
            }
        }
    }
}