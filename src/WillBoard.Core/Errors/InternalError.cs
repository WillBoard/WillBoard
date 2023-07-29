namespace WillBoard.Core.Errors
{
    public class InternalError
    {
        public int Status { get; private set; }
        public string Code { get; private set; }
        public object[] Arguments { get; private set; }

        public InternalError(int status, string code, params object[] arguments)
        {
            Status = status;
            Code = code;
            Arguments = arguments;
        }
    }
}