using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public AuthorizationRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateAsync(Authorization authorization)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = @"INSERT INTO `Authorization` (
                            `AuthorizationId`,
                            `AccountId`,
                            `BoardId`,
                            `Creation`,
                            `Name`,
                            `PermissionReportRead`,
                            `PermissionReportDelete`,
                            `PermissionBanRead`,
                            `PermissionBanCreate`,
                            `PermissionBanUpdate`,
                            `PermissionBanDelete`,
                            `PermissionBanAppealRead`,
                            `PermissionBanAppealAccept`,
                            `PermissionBanAppealReject`,
                            `PermissionIpRead`,
                            `PermissionIpDeletePosts`,
                            `PermissionPostEdit`,
                            `PermissionPostDelete`,
                            `PermissionPostDeleteFile`,
                            `PermissionThreadReplyLock`,
                            `PermissionThreadBumpLock`,
                            `PermissionThreadExcessive`,
                            `PermissionThreadPin`,
                            `PermissionThreadCopy`,
                            `PermissionAuthorizationRead`,
                            `PermissionAuthorizationUpdate`,
                            `PermissionAuthorizationDelete`,
                            `PermissionInvitationRead`,
                            `PermissionInvitationCreate`,
                            `PermissionInvitationUpdate`,
                            `PermissionInvitationDelete`,
                            `PermissionBoardView`,
                            `PermissionBoardUpdate`,
                            `PermissionBoardDelete`
                            ) VALUES (
                            @AuthorizationId,
                            @AccountId,
                            @BoardId,
                            @Creation,
                            @Name,
                            @PermissionReportRead,
                            @PermissionReportDelete,
                            @PermissionBanRead,
                            @PermissionBanCreate,
                            @PermissionBanUpdate,
                            @PermissionBanDelete,
                            @PermissionBanAppealRead,
                            @PermissionBanAppealAccept,
                            @PermissionBanAppealReject,
                            @PermissionIpRead,
                            @PermissionIpDeletePosts,
                            @PermissionPostEdit,
                            @PermissionPostDelete,
                            @PermissionPostDeleteFile,
                            @PermissionThreadReplyLock,
                            @PermissionThreadBumpLock,
                            @PermissionThreadExcessive,
                            @PermissionThreadPin,
                            @PermissionThreadCopy,
                            @PermissionAuthorizationRead,
                            @PermissionAuthorizationUpdate,
                            @PermissionAuthorizationDelete,
                            @PermissionInvitationRead,
                            @PermissionInvitationCreate,
                            @PermissionInvitationUpdate,
                            @PermissionInvitationDelete,
                            @PermissionBoardView,
                            @PermissionBoardUpdate,
                            @PermissionBoardDelete);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, authorization);
            }
        }

        public async Task<IEnumerable<Authorization>> ReadAccountCollectionAsync(Guid accountId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Authorization` WHERE `AccountId` = @AccountId;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Authorization>(sql, new { AccountId = accountId });
            }
        }

        public async Task<Authorization> ReadBoardAsync(string boardId, Guid authorizationId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Authorization` WHERE `BoardId` = @BoardId AND `AuthorizationId` = @AuthorizationId LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<Authorization>(sql, new { BoardId = boardId, AuthorizationId = authorizationId });
            }
        }

        public async Task<IEnumerable<Authorization>> ReadBoardCollectionAsync(string boardId, int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Authorization` WHERE `BoardId` = @BoardId ORDER BY `Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Authorization>(sql, new { BoardId = boardId, Skip = skip, Take = take });
            }
        }

        public async Task<int> ReadBoardCountAsync(string boardId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(`AuthorizationId`) FROM `Authorization` WHERE `BoardId` = @BoardId;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql, new { BoardId = boardId });
            }
        }

        public async Task UpdateAsync(Authorization authorization)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = @"UPDATE `Authorization` SET
                                `Name` = @Name,
                                `PermissionReportRead` = @PermissionReportRead,
                                `PermissionReportDelete` = @PermissionReportDelete,
                                `PermissionBanRead` = @PermissionBanRead,
                                `PermissionBanCreate` = @PermissionBanCreate,
                                `PermissionBanUpdate` = @PermissionBanUpdate,
                                `PermissionBanDelete` = @PermissionBanDelete,
                                `PermissionBanAppealRead` = @PermissionBanAppealRead,
                                `PermissionBanAppealAccept` = @PermissionBanAppealAccept,
                                `PermissionBanAppealReject` = @PermissionBanAppealReject,
                                `PermissionIpRead` = @PermissionIpRead,
                                `PermissionIpDeletePosts` = @PermissionIpDeletePosts,
                                `PermissionPostEdit` = @PermissionPostEdit,
                                `PermissionPostDelete` = @PermissionPostDelete,
                                `PermissionPostDeleteFile` = @PermissionPostDeleteFile,
                                `PermissionThreadReplyLock` = @PermissionThreadReplyLock,
                                `PermissionThreadBumpLock` = @PermissionThreadBumpLock,
                                `PermissionThreadExcessive` = @PermissionThreadExcessive,
                                `PermissionThreadPin` = @PermissionThreadPin,
                                `PermissionThreadCopy` = @PermissionThreadCopy,
                                `PermissionAuthorizationRead` = @PermissionAuthorizationRead,
                                `PermissionAuthorizationUpdate` = @PermissionAuthorizationUpdate,
                                `PermissionAuthorizationDelete` = @PermissionAuthorizationDelete,
                                `PermissionInvitationRead` = @PermissionInvitationRead,
                                `PermissionInvitationCreate` = @PermissionInvitationCreate,
                                `PermissionInvitationUpdate` = @PermissionInvitationUpdate,
                                `PermissionInvitationDelete` = @PermissionInvitationDelete,
                                `PermissionBoardView` = @PermissionBoardView,
                                `PermissionBoardUpdate` = @PermissionBoardUpdate,
                                `PermissionBoardDelete` = @PermissionBoardDelete
                                WHERE `AuthorizationId` = @AuthorizationId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, authorization);
            }
        }

        public async Task DeleteAsync(Guid authorizationId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Authorization` WHERE `AuthorizationId` = @AuthorizationId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { AuthorizationId = authorizationId });
            }
        }
    }
}