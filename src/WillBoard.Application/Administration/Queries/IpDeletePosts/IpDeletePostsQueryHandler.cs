using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.IpDeletePosts
{
    public class IpDeletePostsQueryHandler : IRequestHandler<IpDeletePostsQuery, Result<IpDeletePostsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;

        public IpDeletePostsQueryHandler(AccountManager accountManager, IBoardCache boardCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
        }

        public async Task<Result<IpDeletePostsViewModel, InternalError>> Handle(IpDeletePostsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<IpDeletePostsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (request.IpVersion == IpVersion.None)
            {
                return Result<IpDeletePostsViewModel, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorInvalidIpVersion));
            }

            if (!UInt128.TryParse(request.IpNumber, out UInt128 ipNumber))
            {
                return Result<IpDeletePostsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorInvalidIpNumber));
            }

            var result = new IpDeletePostsViewModel()
            {
                IpVersion = request.IpVersion,
                IpNumber = ipNumber
            };

            return Result<IpDeletePostsViewModel, InternalError>.ValueResult(result);
        }
    }
}