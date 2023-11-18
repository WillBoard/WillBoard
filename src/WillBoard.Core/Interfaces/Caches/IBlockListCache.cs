using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Classes;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IBlockListCache
    {
        Task<bool> GetBoardDnsBlockListIpVersion4Async(string boardId, UInt32 ipNumber, IEnumerable<BlockList> blCollection);
        Task RemoveBoardDnsBlockListIpVersion4Async(string boardId, UInt32 ipNumber);

        Task PurgeBoardDnsBlockListIpVersion4Async(string boardId);

        Task<bool> GetBoardDnsBlockListIpVersion6Async(string boardId, UInt128 ipNumber, IEnumerable<BlockList> blCollection);
        Task RemoveBoardDnsBlockListIpVersion6Async(string boardId, UInt128 ipNumber);

        Task PurgeBoardDnsBlockListIpVersion6Async(string boardId);

        Task<bool> GetBoardApiBlockListIpVersion4Async(string boardId, UInt32 ipNumber, IEnumerable<BlockList> blCollection);
        Task RemoveBoardApiBlockListIpVersion4Async(string boardId, UInt32 ipNumber);

        Task PurgeBoardApiBlockListIpVersion4Async(string boardId);

        Task<bool> GetBoardApiBlockListIpVersion6Async(string boardId, UInt128 ipNumber, IEnumerable<BlockList> blCollection);
        Task RemoveBoardApiBlockListIpVersion6Async(string boardId, UInt128 ipNumber);

        Task PurgeBoardApiBlockListIpVersion6Async(string boardId);
    }
}