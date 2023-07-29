using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Classes;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IBlockListService
    {
        Task<bool> CheckDnsBlockListIpVersion4Async(uint ipNumber, IEnumerable<BlockList> blockListCollection);
        Task<bool> CheckDnsBlockListIpVersion6Async(BigInteger ipNumber, IEnumerable<BlockList> blockListCollection);
        Task<bool> CheckApiBlockListIpVersion4Async(uint ipNumber, IEnumerable<BlockList> blockListCollection);
        Task<bool> CheckApiBlockListIpVersion6Async(BigInteger ipNumber, IEnumerable<BlockList> blockListCollection);
    }
}