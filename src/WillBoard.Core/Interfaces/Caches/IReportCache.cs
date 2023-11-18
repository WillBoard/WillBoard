using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IReportCache
    {
        Task<Report> GetSystemAsync(Guid reportId);
        Task RemoveSystemAsync(Guid reportId);
        Task PurgeSystemAsync();

        Task<IEnumerable<Report>> GetSystemIpCollectionAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task RemoveSystemIpCollectionAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task PurgeSystemIpCollectionAsync();

        Task<IEnumerable<Report>> GetSystemCollectionAsync(int skip, int take);
        Task PurgeSystemCollectionAsync();

        Task<int> GetSystemCountAsync();
        Task RemoveSystemCountAsync();

        Task<Report> GetBoardAsync(string boardId, Guid reportId);
        Task RemoveBoardAsync(string boardId, Guid reportId);
        Task PurgeBoardAsync(string boardId);

        Task<IEnumerable<Report>> GetBoardIpCollectionAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber);
        Task RemoveBoardIpCollectionAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber);
        Task PurgeBoardIpCollectionAsync(string boardId);

        Task<IEnumerable<Report>> GetBoardCollectionAsync(string boardId, int skip, int take);
        Task PurgeBoardCollectionAsync(string boardId);

        Task<int> GetBoardCountAsync(string boardId);
        Task RemoveBoardCountAsync(string boardId);
    }
}