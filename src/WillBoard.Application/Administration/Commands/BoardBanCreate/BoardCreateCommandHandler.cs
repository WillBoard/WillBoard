using MediatR;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;
using WillBoard.Core.Utilities;

namespace WillBoard.Application.Administration.Commands.BoardBanCreate
{
    public class BoardBanCreateCommandHandler : IRequestHandler<BoardBanCreateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBanRepository _banRepository;
        private readonly IBoardCache _boardCache;
        private readonly IBanCache _banCache;

        public BoardBanCreateCommandHandler(AccountManager accountManager, IDateTimeProvider dateTimeProvider, IBanRepository banRepository, IBoardCache boardCache, IBanCache banCache)
        {
            _accountManager = accountManager;
            _dateTimeProvider = dateTimeProvider;
            _banRepository = banRepository;
            _boardCache = boardCache;
            _banCache = banCache;
        }

        public async Task<Status<InternalError>> Handle(BoardBanCreateCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBanCreate))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (request.IpVersion == IpVersion.None)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorInvalidIpVersion));
            }

            if (!BigInteger.TryParse(request.IpNumber, out BigInteger bigInteger))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorInvalidIpNumber));
            }

            var ban = new Ban()
            {
                BanId = Guid.NewGuid(),
                BoardId = request.BoardId,
                Creation = _dateTimeProvider.UtcNow,
                IpVersion = request.IpVersion
            };

            if (ban.IpVersion == IpVersion.IpVersion4)
            {
                var ipNumberRange = IpRange.IpVersion4NumberWithCidrToRange((uint)bigInteger, request.Cidr);
                ban.IpNumberFrom = ipNumberRange[0];
                ban.IpNumberTo = ipNumberRange[1];
            }

            if (ban.IpVersion == IpVersion.IpVersion6)
            {
                var ipNumberRange = IpRange.IpVersion6NumberWithCidrToRange(bigInteger, request.Cidr);
                ban.IpNumberFrom = ipNumberRange[0];
                ban.IpNumberTo = ipNumberRange[1];
            }

            ban.ExclusionIpNumberCollection = ArrayConversion.DeserializeBigInteger(request.ExclusionIpNumberCollection);

            ban.Expiration = request.Expiration;
            ban.Appeal = request.Appeal;
            ban.Reason = request.Reason;
            ban.Note = request.Note;

            await _banRepository.CreateAsync(ban);

            await _banCache.RemoveBoardAsync(board.BoardId, ban.BanId);
            await _banCache.PurgeBoardUnexpiredCollectionAsync(board.BoardId);
            await _banCache.PurgeBoardCollectionAsync(board.BoardId);
            await _banCache.RemoveBoardCountAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}