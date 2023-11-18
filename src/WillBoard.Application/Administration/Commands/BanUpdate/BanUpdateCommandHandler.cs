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

namespace WillBoard.Application.Administration.Commands.BanUpdate
{
    public class BanUpdateCommandHandler : IRequestHandler<BanUpdateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanRepository _banRepository;
        private readonly IBanCache _banCache;

        public BanUpdateCommandHandler(AccountManager accountManager, IBanRepository banRepository, IBanCache banCache)
        {
            _accountManager = accountManager;
            _banRepository = banRepository;
            _banCache = banCache;
        }

        public async Task<Status<InternalError>> Handle(BanUpdateCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var ban = await _banCache.GetSystemAsync(request.BanId);
            if (ban == null || ban.BoardId != null)
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

            await _banCache.RemoveSystemAsync(ban.BanId);
            await _banCache.PurgeSystemUnexpiredCollectionAsync();
            await _banCache.PurgeSystemCollectionAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}