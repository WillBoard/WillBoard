using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public InvitationRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateAsync(Invitation invitation)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Invitation` (`InvitationId`, `AccountId`, `BoardId`, `Creation`, `Message`) VALUES (@InvitationId, @AccountId, @BoardId, @Creation, @Message);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, invitation);
            }
        }

        public async Task<Invitation> ReadAccountAsync(Guid accountId, Guid invitationId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Invitation` WHERE `AccountId` = @AccountId AND `InvitationId` = @InvitationId LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<Invitation>(sql, new { AccountId = accountId, InvitationId = invitationId });
            }
        }

        public async Task<IEnumerable<Invitation>> ReadAccountCollectionAsync(Guid accountId, int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Invitation` WHERE `AccountId` = @AccountId ORDER BY `Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Invitation>(sql, new { AccountId = accountId, Skip = skip, Take = take });
            }
        }

        public async Task<int> ReadAccountCountAsync(Guid accountId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(`InvitationId`) FROM `Invitation` WHERE `AccountId` = @AccountId;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql, new { AccountId = accountId });
            }
        }

        public async Task<Invitation> ReadBoardAsync(string boardId, Guid invitationId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Invitation` WHERE `BoardId` = @BoardId AND `InvitationId` = @InvitationId LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<Invitation>(sql, new { BoardId = boardId, InvitationId = invitationId });
            }
        }

        public async Task<IEnumerable<Invitation>> ReadBoardCollectionAsync(string boardId, int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Invitation` WHERE `BoardId` = @BoardId ORDER BY `Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Invitation>(sql, new { BoardId = boardId, Skip = skip, Take = take });
            }
        }

        public async Task<int> ReadBoardCountAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(`InvitationId`) FROM `Invitation` WHERE `BoardId` = @BoardId;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql, new { BoardId = boardId });
            }
        }

        public async Task UpdateAsync(Invitation invitation)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE `Invitation` SET `AccountId` = @AccountId, `BoardId` = @BoardId, `Message` = @Message WHERE `InvitationId` = @InvitationId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, invitation);
            }
        }

        public async Task DeleteAsync(Guid invitationId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Invitation` WHERE `InvitationId` = @InvitationId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { InvitationId = invitationId });
            }
        }
    }
}