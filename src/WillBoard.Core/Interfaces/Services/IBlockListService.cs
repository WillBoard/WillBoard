using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Classes;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IBlockListService
    {
        Task<bool> CheckDnsBlockListIpVersion4Async(UInt32 ipNumber, IEnumerable<BlockList> blockListCollection);
        Task<bool> CheckDnsBlockListIpVersion6Async(UInt128 ipNumber, IEnumerable<BlockList> blockListCollection);
        Task<bool> CheckApiBlockListIpVersion4Async(UInt32 ipNumber, IEnumerable<BlockList> blockListCollection);
        Task<bool> CheckApiBlockListIpVersion6Async(UInt128 ipNumber, IEnumerable<BlockList> blockListCollection);
    }
}