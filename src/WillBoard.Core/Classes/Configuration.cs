namespace WillBoard.Core.Classes
{
    public class Configuration
    {
        public ConfigurationLogger Logger { get; set; }
        public ConfigurationDatabase Database { get; set; }
        public ConfigurationCache Cache { get; set; }
        public ConfigurationAdministration Administration { get; set; }
    }
}