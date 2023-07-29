using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Commands.BanAppealSystem
{
    public class BanAppealSystemCommandHandler : IRequestHandler<BanAppealSystemCommand, Status<InternalError>>
    {
        private readonly IpManager _ipManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBanRepository _banRepository;
        private readonly IBanAppealRepository _banAppealRepository;
        private readonly IBanCache _banCache;
        private readonly IBanAppealCache _banAppealCache;
        private readonly ILockManager _lockManager;

        public BanAppealSystemCommandHandler(IpManager ipManager, IDateTimeProvider dateTimeProvider, IBanRepository banRepository, IBanAppealRepository banAppealRepository, IBanCache banCache, IBanAppealCache banAppealCache, ILockManager lockManager)
        {
            _ipManager = ipManager;
            _dateTimeProvider = dateTimeProvider;
            _banRepository = banRepository;
            _banAppealRepository = banAppealRepository;
            _banCache = banCache;
            _banAppealCache = banAppealCache;
            _lockManager = lockManager;
        }

        public async Task<Status<InternalError>> Handle(BanAppealSystemCommand request, CancellationToken cancellationToken)
        {
            var ipVersion = _ipManager.GetIpVersion();
            var ipNumber = _ipManager.GetIpNumber();

            if (request.Message.Length < 1)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBanAppealMessageLengthMin));
            }

            if (request.Message.Length > 255)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBanAppealMessageLengthMax));
            }

            var banCacheCollection = await _banCache.GetSystemUnexpiredCollectionAsync(ipVersion, ipNumber);

            var ban = banCacheCollection.FirstOrDefault(b => b.BanId == request.BanId && (b.Expiration == null || b.Expiration > _dateTimeProvider.UtcNow) && !b.ExclusionIpNumberCollection.Contains(ipNumber));
            if (ban == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!ban.Appeal)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBanAppealAvailability));
            }

            var banAppealCollection = await _banAppealCache.GetSystemBanCollectionAsync(ban.BanId);
            var banAppeal = banAppealCollection.FirstOrDefault(e => e.IpVersion == ipVersion && e.IpNumber == ipNumber);

            if (banAppeal != null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
            }

            using (await _lockManager.GetBanAppealLockAsync($"Database_{ban.BanId}"))
            {
                var banRepositoryCollection = await _banRepository.ReadSystemUnexpiredCollectionAsync(ipVersion, ipNumber);
                var banRepository = banRepositoryCollection.FirstOrDefault(b => b.BanId == request.BanId && (b.Expiration == null || b.Expiration > _dateTimeProvider.UtcNow) && !b.ExclusionIpNumberCollection.Contains(ipNumber));
                if (banRepository == null)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
                }

                if (!banRepository.Appeal)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBanAppealAvailability));
                }

                var banAppealRepositoryCollection = await _banAppealRepository.ReadSystemBanCollectionAsync(ban.BanId);
                var banAppealRepository = banAppealRepositoryCollection.FirstOrDefault(e => e.IpVersion == ipVersion && e.IpNumber == ipNumber);

                if (banAppealRepository != null)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
                }

                var newBanAppeal = new BanAppeal()
                {
                    BanAppealId = Guid.NewGuid(),
                    Creation = _dateTimeProvider.UtcNow,
                    IpVersion = ipVersion,
                    IpNumber = ipNumber,
                    Message = request.Message,
                    BanId = ban.BanId
                };

                await _banAppealRepository.CreateAsync(newBanAppeal);

                await _banAppealCache.RemoveSystemAsync(newBanAppeal.BanAppealId);
                await _banAppealCache.RemoveSystemBanCollectionAsync(newBanAppeal.BanId);
                await _banAppealCache.PurgeSystemCollectionAsync();
                await _banAppealCache.RemoveSystemCountAsync();
            }

            return Status<InternalError>.SuccessStatus();
        }
    }
}