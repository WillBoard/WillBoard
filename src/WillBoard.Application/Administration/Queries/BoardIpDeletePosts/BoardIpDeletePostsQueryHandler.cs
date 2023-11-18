using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardIpDeletePosts
{
    public class BoardIpDeletePostsQueryHandler : IRequestHandler<BoardIpDeletePostsQuery, Result<BoardIpDeletePostsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;

        public BoardIpDeletePostsQueryHandler(AccountManager accountManager, IBoardCache boardCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
        }

        public async Task<Result<BoardIpDeletePostsViewModel, InternalError>> Handle(BoardIpDeletePostsQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardIpDeletePostsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (request.IpVersion == IpVersion.None)
            {
                return Result<BoardIpDeletePostsViewModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorInvalidIpVersion));
            }

            if (!UInt128.TryParse(request.IpNumber, out UInt128 ipNumber))
            {
                return Result<BoardIpDeletePostsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorInvalidIpNumber));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionIpDeletePosts))
            {
                return Result<BoardIpDeletePostsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var result = new BoardIpDeletePostsViewModel()
            {
                BoardId = board.BoardId,
                IpVersion = request.IpVersion,
                IpNumber = ipNumber
            };

            return Result<BoardIpDeletePostsViewModel, InternalError>.ValueResult(result);
        }
    }
}