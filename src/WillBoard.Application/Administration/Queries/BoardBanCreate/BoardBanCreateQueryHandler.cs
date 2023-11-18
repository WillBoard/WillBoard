using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardBanCreate
{
    public class BoardBanCreateQueryHandler : IRequestHandler<BoardBanCreateQuery, Result<BoardBanCreateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;

        public BoardBanCreateQueryHandler(IBoardCache boardCache, AccountManager accountManager)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
        }

        public async Task<Result<BoardBanCreateViewModel, InternalError>> Handle(BoardBanCreateQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardBanCreateViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBanCreate))
            {
                return Result<BoardBanCreateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            UInt128.TryParse(request.IPNumber, out UInt128 ipNumber);

            var result = new BoardBanCreateViewModel()
            {
                BoardId = request.BoardId,
                IPVersion = request.IPVersion,
                IPNumber = ipNumber
            };

            return Result<BoardBanCreateViewModel, InternalError>.ValueResult(result);
        }
    }
}