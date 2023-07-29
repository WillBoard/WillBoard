using Dapper;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class BadIpRepository : IBadIpRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public BadIpRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<BadIp> ReadAsync(IpVersion ipVersion, BigInteger ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                // String interpolation is a workaround for converting parameter into a string.
                // The problem still exists even when when DbType is set to VarNumeric in BigIntegerTypeHandler.
                var sql = $"SELECT * FROM `BadIp` WHERE `IpVersion` = @IpVersion AND `IpNumberFrom` <= {ipNumber} AND `IpNumberTo` >= {ipNumber} LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QueryFirstOrDefaultAsync<BadIp>(sql, new { IpVersion = ipVersion, IpNumber = ipNumber });
            }
        }
    }
}