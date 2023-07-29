using System.Threading.Tasks;

namespace WillBoard.Core.Interfaces.Services
{
    public interface ILocalizationService
    {
        Task<string> GetLocalizationAsync(string translationId, string key);
        Task<string> GetLocalizationAsync(string translationId, string key, params object[] arguments);
    }
}