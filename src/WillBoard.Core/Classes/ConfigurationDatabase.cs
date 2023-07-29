using WillBoard.Core.Enums;

namespace WillBoard.Core.Classes
{
    public class ConfigurationDatabase
    {
        public DatabaseType Type { get; set; }
        public string ConnectionString { get; set; }
    }
}