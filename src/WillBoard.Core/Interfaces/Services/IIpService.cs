using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IIpService
    {
        Task<string> GetDnsHostNameAsync(IPAddress ipAddress);
        Task<string> GetCountryIpAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task<bool> GetBadIpAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task<bool> GetDnsBlockListIpVersion4Async(string boardId, UInt32 ipNumber, IEnumerable<BlockList> blockListCollection, bool cache);
        Task<bool> GetDnsBlockListIpVersion6Async(string boardId, UInt128 ipNumber, IEnumerable<BlockList> blockListCollection, bool cache);
        Task<bool> GetApiBlockListIpVersion4Async(string boardId, UInt32 ipNumber, IEnumerable<BlockList> blockListCollection, bool cache);
        Task<bool> GetApiBlockListIpVersion6Async(string boardId, UInt128 ipNumber, IEnumerable<BlockList> blockListCollection, bool cache);
    }
}