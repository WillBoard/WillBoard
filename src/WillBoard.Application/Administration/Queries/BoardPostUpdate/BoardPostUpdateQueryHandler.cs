using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardPostUpdate
{
    public class BoardPostUpdateQueryHandler : IRequestHandler<BoardPostUpdateQuery, Result<BoardPostUpdateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;

        public BoardPostUpdateQueryHandler(AccountManager accountManager, IBoardCache boardCache, IPostCache postCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _postCache = postCache;
        }

        public async Task<Result<BoardPostUpdateViewModel, InternalError>> Handle(BoardPostUpdateQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardPostUpdateViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionPostEdit))
            {
                return Result<BoardPostUpdateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var post = await _postCache.GetAdaptedAsync(board, request.PostId);

            if (post == null)
            {
                return Result<BoardPostUpdateViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new BoardPostUpdateViewModel()
            {
                Post = post
            };

            return Result<BoardPostUpdateViewModel, InternalError>.ValueResult(result);
        }
    }
}