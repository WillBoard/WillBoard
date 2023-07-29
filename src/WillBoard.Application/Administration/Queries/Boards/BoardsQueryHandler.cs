using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Boards
{
    public class BoardsQueryHandler : IRequestHandler<BoardsQuery, Result<BoardsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;

        public BoardsQueryHandler(AccountManager accountManager, IBoardCache boardCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
        }

        public async Task<Result<BoardsViewModel, InternalError>> Handle(BoardsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();
            var requestAuthorizationCollection = _accountManager.GetAuthorizationCollection();

            IEnumerable<Core.Entities.Board> boardCollection = await _boardCache.GetCollectionAsync();

            if (requestAccount.Type != AccountType.Administrator)
            {
                boardCollection = boardCollection.Where(e => requestAuthorizationCollection.Select(a => a.BoardId).Contains(e.BoardId));
            }

            var boardCollectionPage = boardCollection.Skip((request.Page - 1) * 100).Take(100);

            int pageMax = (boardCollection.Count() / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<BoardsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new BoardsViewModel()
            {
                BoardCollection = boardCollectionPage,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<BoardsViewModel, InternalError>.ValueResult(result);
        }
    }
}