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
    public class VerificationRepository : IVerificationRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public VerificationRepository(ISqlConnectionService sqlConnectionService, IDateTimeProvider dateTimeProvider)
        {
            _sqlConnectionService = sqlConnectionService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task CreateAsync(Verification verification)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Verification` (`VerificationId`, `BoardId`, `IpVersion`, `IpNumber`, `Creation`, `Expiration`) VALUES (@VerificationId, @BoardId, @IpVersion, @IpNumber, @Creation, @Expiration);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, verification);
            }
        }

        public async Task<IEnumerable<Verification>> ReadSystemUnexpiredCollectionAsync(IpVersion ipVersion, UInt128 ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Verification` WHERE `BoardId` IS NULL AND `Expiration` >= @ExpirationFrom;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Verification>(sql, new { ExpirationFrom = _dateTimeProvider.UtcNow });
            }
        }

        public async Task<IEnumerable<Verification>> ReadBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Verification` WHERE `BoardId` = @BoardId AND `Expiration` >= @ExpirationFrom;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Verification>(sql, new { BoardId = boardId, ExpirationFrom = _dateTimeProvider.UtcNow });
            }
        }

        public async Task DeleteAsync(Guid verificationId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Verification` WHERE `VerificationId` = @VerificationId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { VerificationId = verificationId });
            }
        }
    }
}