using System;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IBadIpRepository
    {
        Task<BadIp> ReadAsync(IpVersion ipVersion, UInt128 ipNumber);
    }
}