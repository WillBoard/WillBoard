﻿using Dapper;
using System;
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

        public async Task<BadIp> ReadAsync(IpVersion ipVersion, UInt128 ipNumber)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = $"SELECT * FROM `BadIp` WHERE `IpVersion` = @IpVersion AND `IpNumberFrom` <= @IpNumber AND `IpNumberTo` >= @IpNumber LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QueryFirstOrDefaultAsync<BadIp>(sql, new { IpVersion = ipVersion, IpNumber = ipNumber });
            }
        }
    }
}