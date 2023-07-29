using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<Authentication, InternalError>>
    {
        private readonly IpManager _ipManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IConfigurationService _configurationService;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IAccountCache _accountCache;
        private readonly IAuthenticationCache _authenticationCache;
        private readonly IPasswordService _passwordService;
        private readonly IReCaptchaV2Service _reCaptchaV2Service;

        public LoginCommandHandler(
            IpManager ipManager,
            IDateTimeProvider dateTimeProvider,
            IConfigurationService configurationService,
            IAuthenticationRepository authenticationRepository,
            IAccountCache accountCache,
            IAuthenticationCache authenticationCache,
            IPasswordService passwordService,
            IReCaptchaV2Service reCaptchaV2Service)
        {
            _ipManager = ipManager;
            _dateTimeProvider = dateTimeProvider;
            _configurationService = configurationService;
            _authenticationRepository = authenticationRepository;
            _accountCache = accountCache;
            _authenticationCache = authenticationCache;
            _passwordService = passwordService;
            _reCaptchaV2Service = reCaptchaV2Service;
        }

        public async Task<Result<Authentication, InternalError>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (_configurationService.Configuration.Administration.VerificationType == VerificationType.ReCaptcha)
            {
                var verificationResult = await _reCaptchaV2Service.VerifyAsync(_configurationService.Configuration.Administration.VerificationSecretKey, request.VerificationValue);
                if (!verificationResult)
                {
                    return Result<Authentication, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorVerification));
                }
            }

            if (request.Password == null || request.Password.Length < 8 || request.Password.Length > 64)
            {
                return Result<Authentication, InternalError>.ErrorResult(new InternalError(400, TranslationKey.ErrorPassword));
            }

            Account account = await _accountCache.GetAsync(request.AccountId);

            if (account != null)
            {
                if (_passwordService.VerifyPassword(account.Password, request.Password))
                {
                    var authentication = new Authentication()
                    {
                        AuthenticationId = Guid.NewGuid(),
                        AccountId = account.AccountId,
                        IpVersion = _ipManager.GetIpVersion(),
                        IpNumber = _ipManager.GetIpNumber(),
                        Creation = _dateTimeProvider.UtcNow,
                        Expiration = _dateTimeProvider.UtcNow.AddDays(30),
                        Name = request.Name
                    };

                    await _authenticationRepository.CreateAsync(authentication);
                    await _authenticationCache.PurgeCollectionAsync(account.AccountId);

                    return Result<Authentication, InternalError>.ValueResult(authentication);
                }
            }

            return Result<Authentication, InternalError>.ErrorResult(new InternalError(400, "ErrorLogin"));
        }
    }
}