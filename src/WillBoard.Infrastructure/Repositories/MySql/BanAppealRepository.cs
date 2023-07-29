using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class BanAppealRepository : IBanAppealRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public BanAppealRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateAsync(BanAppeal banAppeal)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `BanAppeal` (`BanAppealId`, `Creation`, `IpVersion`, `IpNumber`, `Message`, `BanId`) VALUES (@BanAppealId, @Creation, @IpVersion, @IpNumber, @Message, @BanId);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, banAppeal);
            }
        }

        public async Task<BanAppeal> ReadSystemAsync(Guid banAppealId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT ba.* FROM `BanAppeal` ba INNER JOIN `Ban` b ON ba.`BanId` = b.`BanId` WHERE ba.`BanAppealId` = @BanAppealId AND b.`BoardId` IS NULL LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<BanAppeal>(sql, new { BanAppealId = banAppealId });
            }
        }

        public async Task<IEnumerable<BanAppeal>> ReadSystemBanCollectionAsync(Guid banId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT ba.* FROM `BanAppeal` ba INNER JOIN `Ban` b ON ba.`BanId` = b.`BanId` WHERE ba.`BanId` = @BanId AND b.`BoardId` IS NULL;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<BanAppeal>(sql, new { BanId = banId });
            }
        }

        public async Task<IEnumerable<BanAppeal>> ReadSystemCollectionAsync(int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT ba.*, b.* FROM `BanAppeal` ba INNER JOIN `Ban` b ON ba.`BanId` = b.`BanId` WHERE b.`BoardId` IS NULL ORDER BY ba.`Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<BanAppeal, Ban, BanAppeal>(sql, (banAppeal, ban) => { banAppeal.Ban = ban; return banAppeal; }, new { Skip = skip, Take = take }, splitOn: "BanId");
            }
        }

        public async Task<int> ReadSystemCountAsync()
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(ba.`BanAppealId`) FROM `BanAppeal` ba INNER JOIN `Ban` b ON ba.`BanId` = b.`BanId` WHERE b.`BoardId` IS NULL;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<BanAppeal> ReadBoardAsync(string boardId, Guid banAppealId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT ba.* FROM `BanAppeal` ba INNER JOIN `Ban` b ON ba.`BanId` = b.`BanId` WHERE ba.`BanAppealId` = @BanAppealId AND b.`BoardId` = @BoardId LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<BanAppeal>(sql, new { BoardId = boardId, BanAppealId = banAppealId });
            }
        }

        public async Task<IEnumerable<BanAppeal>> ReadBoardBanCollectionAsync(string boardId, Guid banId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT ba.* FROM `BanAppeal` ba INNER JOIN `Ban` b ON ba.`BanId` = b.`BanId` WHERE ba.`BanId` = @BanId AND b.`BoardId` = @BoardId;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<BanAppeal>(sql, new { BoardId = boardId, BanId = banId });
            }
        }

        public async Task<IEnumerable<BanAppeal>> ReadBoardCollectionAsync(string boardId, int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT ba.*, b.* FROM `BanAppeal` ba INNER JOIN `Ban` b ON ba.`BanId` = b.`BanId` WHERE b.`BoardId` = @BoardId ORDER BY ba.`Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<BanAppeal, Ban, BanAppeal>(sql, (banAppeal, ban) => { banAppeal.Ban = ban; return banAppeal; }, new { BoardId = boardId, Skip = skip, Take = take }, splitOn: "BanId");
            }
        }

        public async Task<int> ReadBoardCountAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(ba.`BanAppealId`) FROM `BanAppeal` ba INNER JOIN `Ban` b ON ba.`BanId` = b.`BanId` WHERE b.`BoardId` = @BoardId;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql, new { BoardId = boardId });
            }
        }

        public async Task UpdateAsync(BanAppeal banBanAppeal)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE `BanAppeal` SET `Creation` = @Creation, `IpVersion` = @IpVersion, `IpNumber` = @IpNumber, `Message` = @Message, `BanId` = @BanId WHERE `BanAppealId` = @BanAppealId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, banBanAppeal);
            }
        }

        public async Task DeleteAsync(Guid banAppealId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `BanAppeal` WHERE `BanAppealId` = @BanAppealId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { BanAppealId = banAppealId });
            }
        }
    }
}