using Microsoft.Extensions.DependencyInjection;
using System;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Infrastructure.BackgroundServices;
using WillBoard.Infrastructure.Locks;
using WillBoard.Infrastructure.Managers;
using WillBoard.Infrastructure.Providers;
using WillBoard.Infrastructure.Services;
using WillBoard.Infrastructure.Services.Instance;

namespace WillBoard.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationService configurationService)
        {
            services.AddHttpClient();

            services.AddMemoryCache();

            services.AddTransient<IDateTimeProvider, DateTimeProvider>();

            switch (configurationService.Configuration.Database.Type)
            {
                case DatabaseType.MySQL:
                    services.AddSingleton<ISqlConnectionService, Repositories.MySqlConnectionService>();

                    services.AddSingleton<IAccountRepository, Repositories.MySql.AccountRepository>();
                    services.AddSingleton<IAuthenticationRepository, Repositories.MySql.AuthenticationRepository>();
                    services.AddSingleton<IAuthorizationRepository, Repositories.MySql.AuthorizationRepository>();
                    services.AddSingleton<IBadIpRepository, Repositories.MySql.BadIpRepository>();
                    services.AddSingleton<IBanAppealRepository, Repositories.MySql.BanAppealRepository>();
                    services.AddSingleton<IBanRepository, Repositories.MySql.BanRepository>();
                    services.AddSingleton<IBoardRepository, Repositories.MySql.BoardRepository>();
                    services.AddSingleton<IConfigurationRepository, Repositories.MySql.ConfigurationRepository>();
                    services.AddSingleton<ICountryIpRepository, Repositories.MySql.CountryIpRepository>();
                    services.AddSingleton<IInvitationRepository, Repositories.MySql.InvitationRepository>();
                    services.AddSingleton<ILogRepository, Repositories.MySql.LogRepository>();
                    services.AddSingleton<INavigationRepository, Repositories.MySql.NavigationRepository>();
                    services.AddSingleton<IPostRepository, Repositories.MySql.PostRepository>();
                    services.AddSingleton<IPostIdentityRepository, Repositories.MySql.PostIdentityRepository>();
                    services.AddSingleton<IPostMentionRepository, Repositories.MySql.PostMentionRepository>();
                    services.AddSingleton<IReportRepository, Repositories.MySql.ReportRepository>();
                    services.AddSingleton<ITranslationRepository, Repositories.MySql.TranslationRepository>();
                    services.AddSingleton<IVerificationRepository, Repositories.MySql.VerificationRepository>();
                    break;

                default:
                    throw new NotSupportedException($@"Database type ""{configurationService.Configuration.Database.Type}"" is not supported.");
            }

            switch (configurationService.Configuration.Cache.Type)
            {
                case CacheType.Memory:
                    services.AddSingleton<ILockManager, MemoryLockManager>();
                    services.AddSingleton<ICancellationTokenManager, MemoryCancellationTokenManager>();

                    services.AddSingleton<IAccountCache, Infrastructure.Caches.Memory.AccountCache>();
                    services.AddSingleton<IAuthenticationCache, Infrastructure.Caches.Memory.AuthenticationCache>();
                    services.AddSingleton<IAuthorizationCache, Infrastructure.Caches.Memory.AuthorizationCache>();
                    services.AddSingleton<IBadIpCache, Infrastructure.Caches.Memory.BadIpCache>();
                    services.AddSingleton<IBanAppealCache, Infrastructure.Caches.Memory.BanAppealCache>();
                    services.AddSingleton<IBanCache, Infrastructure.Caches.Memory.BanCache>();
                    services.AddSingleton<IBoardCache, Infrastructure.Caches.Memory.BoardCache>();
                    services.AddSingleton<IConfigurationCache, Infrastructure.Caches.Memory.ConfigurationCache>();
                    services.AddSingleton<ICountryIpCache, Infrastructure.Caches.Memory.CountryIpCache>();
                    services.AddSingleton<IInvitationCache, Infrastructure.Caches.Memory.InvitationCache>();
                    services.AddSingleton<INavigationCache, Infrastructure.Caches.Memory.NavigationCache>();
                    services.AddSingleton<IPostCache, Infrastructure.Caches.Memory.PostCache>();
                    services.AddSingleton<IReportCache, Infrastructure.Caches.Memory.ReportCache>();
                    services.AddSingleton<ITranslationCache, Infrastructure.Caches.Memory.TranslationCache>();
                    services.AddSingleton<IVerificationCache, Infrastructure.Caches.Memory.VerificationCache>();
                    services.AddSingleton<IBlockListCache, Infrastructure.Caches.Memory.BlockListCache>();
                    break;

                default:
                    throw new NotSupportedException($@"Cache type ""{configurationService.Configuration.Cache.Type}"" is not supported.");
            }

            services.AddTransient<IPasswordService, PasswordService>();

            services.AddTransient<IAuthenticationTokenService, AuthenticationTokenService>();

            services.AddTransient<IStorageService, InstanceStorageService>();

            services.AddTransient<IFFtoolService, InstanceFFtoolService>();

            services.AddSingleton<ISynchronizationService, InstanceSynchronizationService>();

            services.AddSingleton<IOnlineCounterService, InstanceOnlineCounterService>();

            services.AddSingleton<IClassicCaptchaService, InstanceClassicCaptchaService>();

            services.AddTransient<IFileService, FileService>();

            services.AddTransient<IPostService, PostService>();

            services.AddTransient<IVerificationService, VerificationService>();

            services.AddTransient<ILocalizationService, LocalizationService>();

            services.AddTransient<IBlockListService, BlockListService>();

            services.AddTransient<IIpService, IpService>();

            services.AddTransient<IReCaptchaV2Service, ReCaptchaV2Service>();

            services.AddHostedService<ExcessiveBackgroundService>();

            services.AddHostedService<AnonymizationBackgroundService>();

            services.AddHostedService<ModerationBackgroundService>();

            return services;
        }
    }
}