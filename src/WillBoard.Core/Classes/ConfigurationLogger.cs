using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace WillBoard.Core.Classes
{
    public class ConfigurationLogger
    {
        public LogLevel Level { get; set; }
        public Dictionary<string, LogLevel> FilterCollection { get; set; }
    }
}
