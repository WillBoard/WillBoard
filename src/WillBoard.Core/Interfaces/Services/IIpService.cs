using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IIpService
    {
        Task<string> GetDnsHostNameAsync(IPAddress ipAddress);
        Task<string> GetCountryIpAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task<bool> GetBadIpAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task<bool> GetDnsBlockListIpVersion4Async(string boardId, uint ipNumber, IEnumerable<BlockList> blockListCollection, bool cache);
        Task<bool> GetDnsBlockListIpVersion6Async(string boardId, BigInteger ipNumber, IEnumerable<BlockList> blockListCollection, bool cache);
        Task<bool> GetApiBlockListIpVersion4Async(string boardId, uint ipNumber, IEnumerable<BlockList> blockListCollection, bool cache);
        Task<bool> GetApiBlockListIpVersion6Async(string boardId, BigInteger ipNumber, IEnumerable<BlockList> blockListCollection, bool cache);
    }
}