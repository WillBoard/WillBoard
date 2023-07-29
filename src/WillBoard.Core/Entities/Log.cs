using System;

namespace WillBoard.Core.Entities
{
    public class Log
    {
        public Guid LogId { get; set; }
        public DateTime Creation { get; set; }
        public string Message { get; set; }

        public Log()
        {
        }
    }
}