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

namespace WillBoard.Application.Board.Queries.ClassicThread
{
    public class ClassicThreadQueryHandler : IRequestHandler<ClassicThreadQuery, Result<ClassicThreadViewModel, InternalError>>
    {
        private readonly IpManager _ipManager;
        private readonly BoardManager _boardManager;
        private readonly IPostCache _postCache;
        private readonly IVerificationService _verificationService;

        public ClassicThreadQueryHandler(IpManager ipManager, BoardManager boardManager, IPostCache postCache, IVerificationService verificationService)
        {
            _ipManager = ipManager;
            _boardManager = boardManager;
            _postCache = postCache;
            _verificationService = verificationService;
        }

        public async Task<Result<ClassicThreadViewModel, InternalError>> Handle(ClassicThreadQuery request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            var thread = await _postCache.GetAdaptedBoardThreadAsync(board, request.ThreadId, request.Last);

            if (thread.Key == null)
            {
                return Result<ClassicThreadViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var verification = await _verificationService.CheckAsync(false, _ipManager.GetIpVersion(), _ipManager.GetIpNumber(), board);

            var result = new ClassicThreadViewModel()
            {
                BoardViewType = BoardViewType.ClassicThread,
                Title = $"/{board.BoardId}/ - {board.Name}",
                Verification = verification,
                Thread = thread.Key,
                ReplyCollection = thread.Value,
                Last = request.Last
            };

            return Result<ClassicThreadViewModel, InternalError>.ValueResult(result);
        }
    }
}