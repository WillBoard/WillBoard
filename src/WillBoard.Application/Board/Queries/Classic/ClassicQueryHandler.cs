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

namespace WillBoard.Application.Board.Queries.Classic
{
    public class ClassicQueryHandler : IRequestHandler<ClassicQuery, Result<ClassicViewModel, InternalError>>
    {
        private readonly IpManager _ipManager;
        private readonly BoardManager _boardManager;
        private readonly IPostCache _postCache;
        private readonly IVerificationService _verificationService;

        public ClassicQueryHandler(IpManager ipManager, BoardManager boardManager, IPostCache postCache, IVerificationService verificationService)
        {
            _ipManager = ipManager;
            _boardManager = boardManager;
            _postCache = postCache;
            _verificationService = verificationService;
        }

        public async Task<Result<ClassicViewModel, InternalError>> Handle(ClassicQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            if (request.Page < 1)
            {
                return Result<ClassicViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var pageMax = await _postCache.GetAdaptedBoardPageMaxAsync(board);

            if (request.Page > pageMax)
            {
                return Result<ClassicViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var postDictionary = await _postCache.GetAdaptedBoardAsync(board, request.Page);

            var verification = await _verificationService.CheckAsync(true, _ipManager.GetIpVersion(), _ipManager.GetIpNumber(), board);

            var result = new ClassicViewModel()
            {
                BoardViewType = BoardViewType.ClassicBoard,
                Title = $"/{board.BoardId}/ - {board.Name}",
                Verification = verification,
                PostDictionary = postDictionary,
                PageCurrent = request.Page,
                PageMax = pageMax
            };

            return Result<ClassicViewModel, InternalError>.ValueResult(result);
        }
    }
}