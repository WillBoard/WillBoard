using System.Threading.Tasks;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IReCaptchaV2Service
    {
        Task<bool> VerifyAsync(string secret, string value);
    }
}