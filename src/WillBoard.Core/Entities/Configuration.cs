using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class Configuration
    {
        public string Key { get; set; }
        public dynamic Value { get; set; }
        public ConfigurationType Type { get; set; }

        public Configuration()
        {
        }
    }
}