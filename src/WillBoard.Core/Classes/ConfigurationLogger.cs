using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace WillBoard.Core.Classes
{
    public class ConfigurationLogger
    {
        public LogLevel Level { get; set; }
        public Dictionary<string, LogLevel> FilterCollection { get; set; }
    }
}