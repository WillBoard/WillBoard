using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface ICountryIpCache
    {
        Task<CountryIp> GetAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task RemoveAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task PurgeAsync();
    }
}