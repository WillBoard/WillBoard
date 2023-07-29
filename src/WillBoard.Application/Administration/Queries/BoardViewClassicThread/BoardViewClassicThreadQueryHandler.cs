using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardViewClassicThread
{
    public class BoardViewClassicThreadQueryHandler : IRequestHandler<BoardViewClassicThreadQuery, Result<BoardViewClassicThreadViewModel, InternalError>>
    {
        private readonly IpManager _ipManager;
        private readonly AccountManager _accountManager;
        private readonly BoardManager _boardManager;
        private readonly IPostCache _postCache;
        private readonly IVerificationService _verificationService;

        public BoardViewClassicThreadQueryHandler(IpManager ipManager, AccountManager accountManager, BoardManager boardManager, IPostCache postCache, IVerificationService verificationService)
        {
            _ipManager = ipManager;
            _accountManager = accountManager;
            _boardManager = boardManager;
            _postCache = postCache;
            _verificationService = verificationService;
        }

        public async Task<Result<BoardViewClassicThreadViewModel, InternalError>> Handle(BoardViewClassicThreadQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            if (!_accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardView))
            {
                return Result<BoardViewClassicThreadViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var thread = await _postCache.GetAdaptedBoardThreadAsync(board, request.ThreadId, request.Last);

            if (thread.Key == null)
            {
                return Result<BoardViewClassicThreadViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var verification = await _verificationService.CheckAsync(false, _ipManager.GetIpVersion(), _ipManager.GetIpNumber(), board);

            var result = new BoardViewClassicThreadViewModel()
            {
                BoardViewType = BoardViewType.ClassicThread,
                Title = $"/{board.BoardId}/ - {board.Name}",
                Verification = verification,
                Thread = thread.Key,
                ReplyCollection = thread.Value,
                Last = request.Last
            };

            return Result<BoardViewClassicThreadViewModel, InternalError>.ValueResult(result);
        }
    }
}