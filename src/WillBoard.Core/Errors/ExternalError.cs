namespace WillBoard.Core.Errors
{
    public class ExternalError
    {
        public int Status { get; private set; }
        public string Code { get; private set; }
        public string Message { get; private set; }

        public ExternalError(int status, string code, string message)
        {
            Status = status;
            Code = code;
            Message = message;
        }
    }
}