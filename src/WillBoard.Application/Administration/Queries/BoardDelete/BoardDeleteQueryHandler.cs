using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardDelete
{
    public class BoardDeleteQueryHandler : IRequestHandler<BoardDeleteQuery, Result<BoardDeleteViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;

        public BoardDeleteQueryHandler(AccountManager accountManager, IBoardCache boardCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
        }

        public async Task<Result<BoardDeleteViewModel, InternalError>> Handle(BoardDeleteQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardDeleteViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBoardDelete))
            {
                return Result<BoardDeleteViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var result = new BoardDeleteViewModel()
            {
                Board = board
            };

            return Result<BoardDeleteViewModel, InternalError>.ValueResult(result);
        }
    }
}