using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;
using WillBoard.Core.Utilities;

namespace WillBoard.Application.Administration.Queries.BoardIp
{
    public class BoardIpQueryHandler : IRequestHandler<BoardIpQuery, Result<BoardIpViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;
        private readonly IIpService _ipService;

        public BoardIpQueryHandler(AccountManager accountManager, IBoardCache boardCache, IPostCache postCache, IIpService ipService)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _postCache = postCache;
            _ipService = ipService;
        }

        public async Task<Result<BoardIpViewModel, InternalError>> Handle(BoardIpQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardIpViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (request.IpVersion == IpVersion.None)
            {
                return Result<BoardIpViewModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorInvalidIpVersion));
            }

            if (!UInt128.TryParse(request.IpNumber, out UInt128 ipNumber))
            {
                return Result<BoardIpViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorInvalidIpNumber));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionIpRead))
            {
                return Result<BoardIpViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var postCollection = await _postCache.GetAdaptedCollectionAsync(board);

            var ipPostCollection = postCollection.Where(e => e.IpVersion == request.IpVersion && e.IpNumber == ipNumber).OrderByDescending(e => e.PostId);

            var ipAddress = IpConversion.IpNumberToIpAddress(request.IpVersion, ipNumber);
            var dns = await _ipService.GetDnsHostNameAsync(ipAddress);

            var country = await _ipService.GetCountryIpAsync(request.IpVersion, ipNumber);

            var result = new BoardIpViewModel()
            {
                PostCollection = ipPostCollection,
                IpVersion = request.IpVersion,
                IpNumber = ipNumber,
                Dns = dns,
                Country = country
            };

            return Result<BoardIpViewModel, InternalError>.ValueResult(result);
        }
    }
}