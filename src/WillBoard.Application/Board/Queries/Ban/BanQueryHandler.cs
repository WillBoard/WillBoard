using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.Ban
{
    public class BanQueryHandler : IRequestHandler<BanQuery, Result<BanViewModel, InternalError>>
    {
        private readonly IpManager _ipManager;
        private readonly BoardManager _boardManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBanCache _banCache;
        private readonly IBanAppealCache _banAppealCache;

        public BanQueryHandler(IpManager ipManager, BoardManager boardManager, IDateTimeProvider dateTimeProvider, IBanCache banCache, IBanAppealCache banAppealCache)
        {
            _ipManager = ipManager;
            _boardManager = boardManager;
            _dateTimeProvider = dateTimeProvider;
            _banCache = banCache;
            _banAppealCache = banAppealCache;
        }

        public async Task<Result<BanViewModel, InternalError>> Handle(BanQuery request, CancellationToken cancellationToken)
        {
            var ipVersion = _ipManager.GetIpVersion();
            var ipNumber = _ipManager.GetIpNumber();

            var board = _boardManager.GetBoard();

            var banCollection = await _banCache.GetSystemUnexpiredCollectionAsync(ipVersion, ipNumber);

            var banBoardCollection = await _banCache.GetBoardUnexpiredCollectionAsync(board.BoardId, ipVersion, ipNumber);

            var activeBanCollection = banCollection.Where(b => (b.Expiration == null || b.Expiration > _dateTimeProvider.UtcNow) && !b.ExclusionIpNumberCollection.Contains(ipNumber));
            var activeBanBanCollection = banBoardCollection.Where(b => (b.Expiration == null || b.Expiration > _dateTimeProvider.UtcNow) && !b.ExclusionIpNumberCollection.Contains(ipNumber));

            var banDictionary = new Dictionary<WillBoard.Core.Entities.Ban, BanAppeal>();

            foreach (var ban in activeBanCollection)
            {
                var banAppealCollection = await _banAppealCache.GetSystemBanCollectionAsync(ban.BanId);
                var banAppeal = banAppealCollection.FirstOrDefault(e => e.IpVersion == ipVersion && e.IpNumber == ipNumber);
                banDictionary.Add(ban, banAppeal);
            }

            foreach (var ban in activeBanBanCollection)
            {
                var banAppealCollection = await _banAppealCache.GetBoardBanCollectionAsync(board.BoardId, ban.BanId);
                var banAppeal = banAppealCollection.FirstOrDefault(e => e.IpVersion == ipVersion && e.IpNumber == ipNumber);
                banDictionary.Add(ban, banAppeal);
            }

            var result = new BanViewModel()
            {
                BoardViewType = BoardViewType.Other,
                Title = $"/{board.BoardId}/ - {board.Name}",
                BanDictionary = banDictionary
            };

            return Result<BanViewModel, InternalError>.ValueResult(result);
        }
    }
}