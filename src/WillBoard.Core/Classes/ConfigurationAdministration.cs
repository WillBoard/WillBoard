using WillBoard.Core.Enums;

namespace WillBoard.Core.Classes
{
    public class ConfigurationAdministration
    {
        public VerificationType VerificationType { get; set; }
        public string VerificationPublicKey { get; set; }
        public string VerificationSecretKey { get; set; }
    }
}