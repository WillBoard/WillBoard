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

namespace WillBoard.Application.Administration.Queries.BoardViewClassic
{
    public class BoardViewClassicQueryHandler : IRequestHandler<BoardViewClassicQuery, Result<BoardViewClassicViewModel, InternalError>>
    {
        private readonly IpManager _ipManager;
        private readonly AccountManager _accountManager;
        private readonly BoardManager _boardManager;
        private readonly IPostCache _postCache;
        private readonly IVerificationService _verificationService;

        public BoardViewClassicQueryHandler(IpManager ipManager, AccountManager accountManager, BoardManager boardManager, IPostCache postCache, IVerificationService verificationService)
        {
            _ipManager = ipManager;
            _accountManager = accountManager;
            _boardManager = boardManager;
            _postCache = postCache;
            _verificationService = verificationService;
        }

        public async Task<Result<BoardViewClassicViewModel, InternalError>> Handle(BoardViewClassicQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            if (!_accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardView))
            {
                return Result<BoardViewClassicViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (request.Page < 1)
            {
                return Result<BoardViewClassicViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var pageMax = await _postCache.GetAdaptedBoardPageMaxAsync(board);

            if (request.Page > pageMax)
            {
                return Result<BoardViewClassicViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var postDictionary = await _postCache.GetAdaptedBoardAsync(board, request.Page);

            var verification = await _verificationService.CheckAsync(true, _ipManager.GetIpVersion(), _ipManager.GetIpNumber(), board);

            var result = new BoardViewClassicViewModel()
            {
                BoardViewType = BoardViewType.ClassicBoard,
                Title = $"/{board.BoardId}/ - {board.Name}",
                Verification = verification,
                PostDictionary = postDictionary,
                PageCurrent = request.Page,
                PageMax = pageMax
            };

            return Result<BoardViewClassicViewModel, InternalError>.ValueResult(result);
        }
    }
}