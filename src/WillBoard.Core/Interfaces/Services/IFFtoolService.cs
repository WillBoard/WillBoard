using System.IO;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Results;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IFFtoolService
    {
        Task<Result<FFinformation, string>> FFprobeAsync(Stream inputStream);
        Task<Result<string, string>> FFprobeAsync(Stream inputStream, string outputArguments);
        Task<Result<string, string>> FFmpegAsync(Stream inputStream, string outputArguments);
    }
}