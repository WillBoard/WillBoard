using System;

namespace WillBoard.Core.Exceptions
{
    public class ErrorException : Exception
    {
        public int Status { get; }
        public string Code { get; }

        private ErrorException()
        {
        }

        public ErrorException(int status, string name)
        {
            Status = status;
            Code = name;
        }

        public ErrorException(int status, string code, string message) : base(message)
        {
            Status = status;
            Code = code;
        }

        public ErrorException(int status, string code, string message, Exception innerException) : base(message, innerException)
        {
            Status = status;
            Code = code;
        }
    }
}