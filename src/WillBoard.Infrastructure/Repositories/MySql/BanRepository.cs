using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class BanRepository : IBanRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public BanRepository(ISqlConnectionService sqlConnectionService, IDateTimeProvider dateTimeProvider)
        {
            _sqlConnectionService = sqlConnectionService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task CreateAsync(Ban ban)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Ban` (`BanId`, `BoardId`, `Creation`, `Expiration`, `Appeal`, `IpVersion`, `IpNumberFrom`, `IpNumberTo`, `ExclusionIpNumberCollection`, `Reason`, `Note`) VALUES (@BanId, @BoardId, @Creation, @Expiration, @Appeal, @IpVersion, @IpNumberFrom, @IpNumberTo, @ExclusionIpNumberCollection, @Reason, @Note);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, ban);
            }
        }

        public async Task<Ban> ReadSystemAsync(Guid banId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Ban` WHERE `BoardId` IS NULL AND `BanId` = @BanId LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<Ban>(sql, new { BanId = banId });
            }
        }

        public async Task<IEnumerable<Ban>> ReadSystemUnexpiredCollectionAsync(IpVersion ipVersion, UInt128 ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Ban` WHERE `BoardId` IS NULL AND (`Expiration` >= @ExpirationFrom OR `Expiration` IS NULL) AND `IpVersion` = @IpVersion AND `IpNumberFrom` <= @IpNumber AND `IpNumberTo` >= @IpNumber;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Ban>(sql, new { ExpirationFrom = _dateTimeProvider.UtcNow, IpVersion = ipVersion, IpNumber = ipNumber });
            }
        }

        public async Task<IEnumerable<Ban>> ReadSystemCollectionAsync(int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Ban` WHERE `BoardId` IS NULL ORDER BY `Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Ban>(sql, new { Skip = skip, Take = take });
            }
        }

        public async Task<int> ReadSystemCountAsync()
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(`BanId`) FROM `Ban` WHERE `BoardId` IS NULL;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<Ban> ReadBoardAsync(string boardId, Guid banId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Ban` WHERE `BoardId` = @BoardId AND `BanId` = @BanId LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<Ban>(sql, new { BanId = banId, BoardId = boardId });
            }
        }

        public async Task<IEnumerable<Ban>> ReadBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Ban` WHERE `BoardId` = @BoardId AND (`Expiration` >= @ExpirationFrom OR `Expiration` IS NULL) AND `IpVersion` = @IpVersion AND `IpNumberFrom` <= @IpNumber AND `IpNumberTo` >= @IpNumber;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Ban>(sql, new { boardId, ExpirationFrom = _dateTimeProvider.UtcNow, IpVersion = ipVersion, IpNumber = ipNumber });
            }
        }

        public async Task<IEnumerable<Ban>> ReadBoardCollectionAsync(string boardId, int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Ban` WHERE `BoardId` = @BoardId ORDER BY `Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Ban>(sql, new { BoardId = boardId, Skip = skip, Take = take });
            }
        }

        public async Task<int> ReadBoardCountAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(`BanId`) FROM `Ban` WHERE `BoardId` = @BoardId;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql, new { BoardId = boardId });
            }
        }

        public async Task UpdateAsync(Ban ban)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE `Ban` SET `Expiration` = @Expiration, `Appeal` = @Appeal, `IpVersion` = @IpVersion, `IpNumberFrom` = @IpNumberFrom, `IpNumberTo` = @IpNumberTo, `ExclusionIpNumberCollection` = @ExclusionIpNumberCollection, `Reason` = @Reason, `Note` = @Note WHERE `BanId` = @BanId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, ban);
            }
        }

        public async Task DeleteAsync(Guid banId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Ban` WHERE `BanId` = @BanId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { BanId = banId });
            }
        }
    }
}