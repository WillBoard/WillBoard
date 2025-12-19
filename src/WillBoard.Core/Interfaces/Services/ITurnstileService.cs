using System.Threading.Tasks;

namespace WillBoard.Core.Interfaces.Services
{
    public interface ITurnstileService
    {
        Task<bool> VerifyAsync(string secret, string value);
    }
}
