using System;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface ICountryIpCache
    {
        Task<CountryIp> GetAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task RemoveAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task PurgeAsync();
    }
}