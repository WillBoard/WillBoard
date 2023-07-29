using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class CountryIpRepository : ICountryIpRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public CountryIpRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateCollectionAsync(IEnumerable<CountryIp> countryIpCollection)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                await CreateCollectionAsync(countryIpCollection, dbConnection, null);
            }
        }

        public async Task CreateCollectionAsync(IEnumerable<CountryIp> countryIpCollection, IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = "INSERT INTO `CountryIp` (`IpVersion`, `IpNumberFrom`, `IpNumberTo`, `CountryCode`) VALUES (@IpVersion, @IpNumberFrom, @IpNumberTo, @CountryCode);";

            await dbConnection.ExecuteAsync(sql, countryIpCollection, transaction: transaction);
        }

        public async Task<CountryIp> ReadAsync(IpVersion ipVersion, BigInteger ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                // String interpolation is a workaround for converting parameter into a string.
                // The problem still exists even when when DbType is set to VarNumeric in BigIntegerTypeHandler.
                var sql = $"SELECT * FROM `CountryIp` WHERE `IpVersion` = @IpVersion AND `IpNumberFrom` <= {ipNumber} AND `IpNumberTo` >= {ipNumber} LIMIT 1;";

                return await dbConnection.QueryFirstOrDefaultAsync<CountryIp>(sql, new { IpVersion = ipVersion, IpNumber = ipNumber });
            }
        }

        public async Task TruncateAsync()
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                await TruncateAsync(dbConnection, null);
            }
        }

        public async Task TruncateAsync(IDbConnection dbConnection, IDbTransaction transaction)
        {
            var sql = "TRUNCATE TABLE `CountryIp`;";

            await dbConnection.ExecuteAsync(sql, transaction: transaction);
        }
    }
}