using System;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IBadIpCache
    {
        Task<BadIp> GetAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task RemoveAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task PurgeAsync();
    }
}