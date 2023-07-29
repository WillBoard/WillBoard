using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task CreateAsync(Report report);

        Task<Report> ReadSystemAsync(Guid reportId);
        Task<IEnumerable<Report>> ReadSystemIPCollectionAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task<IEnumerable<Report>> ReadSystemCollectionAsync(int skip, int take);
        Task<int> ReadSystemCountAsync();

        Task<Report> ReadBoardAsync(string boardId, Guid reportId);
        Task<IEnumerable<Report>> ReadBoardIPCollectionAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber);
        Task<IEnumerable<Report>> ReadBoardCollectionAsync(string boardId, int skip, int take);
        Task<int> ReadBoardCountAsync(string boardId);

        Task DeleteAsync(Guid reportId);
    }
}