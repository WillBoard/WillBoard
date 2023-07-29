using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Infrastructure.Services;
using WillBoard.Web.Models;

namespace WillBoard.Web.Filters
{
    public class AuthenticationFilterAttribute : TypeFilterAttribute
    {
        public AuthenticationFilterAttribute() : base(typeof(AuthenticationFilter))
        {
        }

        private class AuthenticationFilter : IAsyncAuthorizationFilter
        {
            private readonly IpManager _ipManager;
            private readonly AccountManager _accountManager;
            private readonly IDateTimeProvider _dateTimeProvider;
            private readonly IAccountCache _accountCache;
            private readonly IAuthenticationCache _authenticationCache;
            private readonly IAuthorizationCache _authorizationCache;
            private readonly IAuthenticationTokenService _authenticationTokenService;
            private readonly ILocalizationService _localizationService;
            private readonly JsonService _jsonService;

            public AuthenticationFilter(
                IpManager ipManager,
                AccountManager accountManager,
                IDateTimeProvider dateTimeProvider,
                IAccountCache accountCache,
                IAuthenticationCache authenticationCache,
                IAuthorizationCache authorizationCache,
                IAuthenticationTokenService authenticationTokenService,
                ILocalizationService localizationService,
                JsonService jsonService)
            {
                _ipManager = ipManager;
                _accountManager = accountManager;
                _dateTimeProvider = dateTimeProvider;
                _authenticationTokenService = authenticationTokenService;
                _accountCache = accountCache;
                _authenticationCache = authenticationCache;
                _authorizationCache = authorizationCache;
                _localizationService = localizationService;
                _jsonService = jsonService;
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                var authenticationCookie = context.HttpContext.Request.Cookies["authentication"];

                if (authenticationCookie != null && authenticationCookie.Length == 44)
                {
                    if (_authenticationTokenService.TryDecode(authenticationCookie, out Guid accountId, out Guid guid))
                    {
                        var account = await _accountCache.GetAsync(accountId);
                        if (account != null && account.Active)
                        {
                            var authentication = await _authenticationCache.GetAsync(accountId, guid);
                            if (authentication != null)
                            {
                                if (authentication.Expiration > _dateTimeProvider.UtcNow)
                                {
                                    var ipVersion = _ipManager.GetIpVersion();
                                    var ipNumber = _ipManager.GetIpNumber();

                                    if (authentication.IpVersion == ipVersion && authentication.IpNumber == ipNumber)
                                    {
                                        _accountManager.SetAccount(account);
                                        _accountManager.SetAuthentication(authentication);

                                        var authorizationCollection = await _authorizationCache.GetAccountCollectionAsync(account.AccountId);
                                        _accountManager.SetAuthorizationCollection(authorizationCollection);

                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                context.HttpContext.Response.Cookies.Delete("authentication");

                var endpoint = context.HttpContext.GetEndpoint();

                if (endpoint == null)
                {
                    context.Result = new RedirectToActionResult("Login", "Administration", null);
                    return;
                }

                var endpointResponse = endpoint.Metadata.GetMetadata<EndpointResponse>();

                if (endpointResponse == null)
                {
                    context.Result = new RedirectToActionResult("Login", "Administration", null);
                    return;
                }

                if (endpointResponse.ContentType == EndpointContentType.JSON)
                {
                    var message = await _localizationService.GetLocalizationAsync("en", TranslationKey.ErrorUnauthorized);
                    var externalError = new ExternalError(401, TranslationKey.ErrorUnauthorized, message);

                    context.Result = new ContentResult()
                    {
                        StatusCode = 401,
                        ContentType = ContentType.ApplicationJson,
                        Content = _jsonService.SerializeError(externalError)
                    };
                }
                else
                {
                    context.Result = new RedirectToActionResult("Login", "Administration", null);
                }
            }
        }
    }
}