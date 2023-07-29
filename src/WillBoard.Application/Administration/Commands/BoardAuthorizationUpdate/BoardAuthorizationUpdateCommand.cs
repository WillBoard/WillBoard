using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardAuthorizationUpdate
{
    public class BoardAuthorizationUpdateCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public Guid AuthorizationId { get; set; }

        public string Name { get; set; }

        public bool PermissionReportRead { get; set; }
        public bool PermissionReportDelete { get; set; }

        public bool PermissionBanRead { get; set; }
        public bool PermissionBanCreate { get; set; }
        public bool PermissionBanUpdate { get; set; }
        public bool PermissionBanDelete { get; set; }

        public bool PermissionBanAppealRead { get; set; }
        public bool PermissionBanAppealAccept { get; set; }
        public bool PermissionBanAppealReject { get; set; }

        public bool PermissionIpRead { get; set; }
        public bool PermissionIpDeletePosts { get; set; }

        public bool PermissionPostEdit { get; set; }
        public bool PermissionPostDelete { get; set; }
        public bool PermissionPostDeleteFile { get; set; }

        public bool PermissionThreadReplyLock { get; set; }
        public bool PermissionThreadBumpLock { get; set; }
        public bool PermissionThreadExcessive { get; set; }
        public bool PermissionThreadPin { get; set; }
        public bool PermissionThreadCopy { get; set; }

        public bool PermissionAuthorizationRead { get; set; }
        public bool PermissionAuthorizationUpdate { get; set; }
        public bool PermissionAuthorizationDelete { get; set; }

        public bool PermissionInvitationRead { get; set; }
        public bool PermissionInvitationCreate { get; set; }
        public bool PermissionInvitationUpdate { get; set; }
        public bool PermissionInvitationDelete { get; set; }

        public bool PermissionBoardView { get; set; }
        public bool PermissionBoardUpdate { get; set; }
        public bool PermissionBoardDelete { get; set; }
    }
}