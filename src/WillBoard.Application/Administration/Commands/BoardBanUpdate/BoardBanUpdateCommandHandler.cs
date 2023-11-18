using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;
using WillBoard.Core.Utilities;

namespace WillBoard.Application.Administration.Commands.BoardBanUpdate
{
    public class BoardBanUpdateCommandHandler : IRequestHandler<BoardBanUpdateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanRepository _banRepository;
        private readonly IBoardCache _boardCache;
        private readonly IBanCache _banCache;

        public BoardBanUpdateCommandHandler(AccountManager accountManager, IBanRepository banRepository, IBoardCache boardCache, IBanCache banCache)
        {
            _accountManager = accountManager;
            _banRepository = banRepository;
            _boardCache = boardCache;
            _banCache = banCache;
        }

        public async Task<Status<InternalError>> Handle(BoardBanUpdateCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);
            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBanUpdate))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var ban = await _banCache.GetBoardAsync(request.BoardId, request.BanId);
            if (ban == null || ban.BoardId != request.BoardId)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (request.IpVersion == IpVersion.None)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorInvalidIpVersion));
            }

            if (!UInt128.TryParse(request.IpNumberFrom, out UInt128 ipNumberFrom))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorInvalidIpNumberFrom));
            }

            if (!UInt128.TryParse(request.IpNumberTo, out UInt128 ipNumberTo))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorInvalidIpNumberTo));
            }

            var banUpdate = new Ban()
            {
                BanId = ban.BanId,
                BoardId = ban.BoardId,
                Expiration = request.Expiration,
                Appeal = request.Appeal,

                IpVersion = request.IpVersion,
                IpNumberFrom = ipNumberFrom,
                IpNumberTo = ipNumberTo,

                ExclusionIpNumberCollection = ArrayConversion.DeserializeUInt128(request.ExclusionIpNumberCollection),

                Reason = request.Reason,
                Note = request.Note
            };

            await _banRepository.UpdateAsync(banUpdate);

            await _banCache.RemoveBoardAsync(board.BoardId, ban.BanId);
            await _banCache.PurgeBoardUnexpiredCollectionAsync(board.BoardId);
            await _banCache.PurgeBoardCollectionAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}