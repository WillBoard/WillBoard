using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class ReportRepository : IReportRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public ReportRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateAsync(Report report)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Report` (`ReportId`, `BoardId`, `Creation`, `ReferenceBoardId`, `ReferencePostId`, `IpVersion`, `IpNumber`, `Reason`) VALUES (@ReportId, @BoardId, @Creation, @ReferenceBoardId, @ReferencePostId, @IpVersion, @IpNumber, @Reason);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, report);
            }
        }

        public async Task<Report> ReadSystemAsync(Guid reportId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Report` WHERE `BoardId` IS NULL AND `ReportId` = @ReportId  LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<Report>(sql, new { ReportId = reportId });
            }
        }

        public async Task<IEnumerable<Report>> ReadSystemIPCollectionAsync(IpVersion ipVersion, UInt128 ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Report` WHERE `BoardId` IS NULL AND `IpVersion` = @IpVersion AND `IpNumber` = @IpNumber;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Report>(sql, new { IpVersion = ipVersion, IpNumber = ipNumber });
            }
        }

        public async Task<IEnumerable<Report>> ReadSystemCollectionAsync(int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Report` WHERE `BoardId` IS NULL ORDER BY `Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Report>(sql, new { Skip = skip, Take = take });
            }
        }

        public async Task<int> ReadSystemCountAsync()
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(`ReportId`) FROM `Report` WHERE `BoardId` IS NULL;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<Report> ReadBoardAsync(string boardId, Guid reportId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Report` WHERE `BoardId` = @BoardId AND `ReportId` = @ReportId  LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<Report>(sql, new { ReportId = reportId, BoardId = boardId });
            }
        }

        public async Task<IEnumerable<Report>> ReadBoardIPCollectionAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Report` WHERE `BoardId` = @BoardId AND `IpVersion` = @IpVersion AND `IpNumber` = @IpNumber;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Report>(sql, new { BoardId = boardId, IpVersion = ipVersion, IpNumber = ipNumber });
            }
        }

        public async Task<IEnumerable<Report>> ReadBoardCollectionAsync(string boardId, int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Report` WHERE `BoardId` = @BoardId ORDER BY `Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Report>(sql, new { BoardId = boardId, Skip = skip, Take = take });
            }
        }

        public async Task<int> ReadBoardCountAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(`ReportId`) FROM `Report` WHERE `BoardId` = @BoardId;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql, new { BoardId = boardId });
            }
        }

        public async Task DeleteAsync(Guid reportId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Report` WHERE `ReportId` = @ReportId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { ReportId = reportId });
            }
        }
    }
}