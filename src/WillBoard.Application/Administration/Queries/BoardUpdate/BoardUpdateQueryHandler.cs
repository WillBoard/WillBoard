using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardUpdate
{
    public class BoardUpdateQueryHandler : IRequestHandler<BoardUpdateQuery, Result<BoardUpdateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;

        public BoardUpdateQueryHandler(AccountManager accountManager, IBoardCache boardCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
        }

        public async Task<Result<BoardUpdateViewModel, InternalError>> Handle(BoardUpdateQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardUpdateViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBoardUpdate))
            {
                return Result<BoardUpdateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var result = new BoardUpdateViewModel()
            {
                Board = board
            };

            return Result<BoardUpdateViewModel, InternalError>.ValueResult(result);
        }
    }
}