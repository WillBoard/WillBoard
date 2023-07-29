using WillBoard.Core.Classes;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IConfigurationService
    {
        Configuration Configuration { get; }

        void SetConfiguration();
    }
}