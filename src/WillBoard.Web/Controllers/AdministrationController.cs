using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WillBoard.Application.Administration.Commands.AccountAuthenticationDeactivate;
using WillBoard.Application.Administration.Commands.AccountCreate;
using WillBoard.Application.Administration.Commands.AccountInvitationAccept;
using WillBoard.Application.Administration.Commands.AccountInvitationReject;
using WillBoard.Application.Administration.Commands.AccountPasswordChange;
using WillBoard.Application.Administration.Commands.AccountUpdate;
using WillBoard.Application.Administration.Commands.BanAppealAccept;
using WillBoard.Application.Administration.Commands.BanAppealReject;
using WillBoard.Application.Administration.Commands.BanCreate;
using WillBoard.Application.Administration.Commands.BanDelete;
using WillBoard.Application.Administration.Commands.BanUpdate;
using WillBoard.Application.Administration.Commands.BoardAuthorizationDelete;
using WillBoard.Application.Administration.Commands.BoardAuthorizationUpdate;
using WillBoard.Application.Administration.Commands.BoardBanAppealAccept;
using WillBoard.Application.Administration.Commands.BoardBanAppealReject;
using WillBoard.Application.Administration.Commands.BoardBanCreate;
using WillBoard.Application.Administration.Commands.BoardBanDelete;
using WillBoard.Application.Administration.Commands.BoardBanUpdate;
using WillBoard.Application.Administration.Commands.BoardCreate;
using WillBoard.Application.Administration.Commands.BoardDelete;
using WillBoard.Application.Administration.Commands.BoardInvitationCreate;
using WillBoard.Application.Administration.Commands.BoardInvitationDelete;
using WillBoard.Application.Administration.Commands.BoardIpDeletePosts;
using WillBoard.Application.Administration.Commands.BoardPostDelete;
using WillBoard.Application.Administration.Commands.BoardPostDeleteFile;
using WillBoard.Application.Administration.Commands.BoardPostUpdate;
using WillBoard.Application.Administration.Commands.BoardReportDelete;
using WillBoard.Application.Administration.Commands.BoardThreadBumpLock;
using WillBoard.Application.Administration.Commands.BoardThreadCopy;
using WillBoard.Application.Administration.Commands.BoardThreadExcessive;
using WillBoard.Application.Administration.Commands.BoardThreadPin;
using WillBoard.Application.Administration.Commands.BoardThreadReplyLock;
using WillBoard.Application.Administration.Commands.BoardUpdate;
using WillBoard.Application.Administration.Commands.CacheClear;
using WillBoard.Application.Administration.Commands.ConfigurationCreate;
using WillBoard.Application.Administration.Commands.ConfigurationDelete;
using WillBoard.Application.Administration.Commands.ConfigurationUpdate;
using WillBoard.Application.Administration.Commands.Login;
using WillBoard.Application.Administration.Commands.Logout;
using WillBoard.Application.Administration.Commands.NavigationCreate;
using WillBoard.Application.Administration.Commands.NavigationDelete;
using WillBoard.Application.Administration.Commands.NavigationUpdate;
using WillBoard.Application.Administration.Commands.ReportDelete;
using WillBoard.Application.Administration.Commands.TranslationCreate;
using WillBoard.Application.Administration.Commands.TranslationDelete;
using WillBoard.Application.Administration.Commands.TranslationUpdate;
using WillBoard.Application.Administration.Queries.Account;
using WillBoard.Application.Administration.Queries.AccountAuthentications;
using WillBoard.Application.Administration.Queries.AccountCreate;
using WillBoard.Application.Administration.Queries.AccountInvitations;
using WillBoard.Application.Administration.Queries.AccountPasswordChange;
using WillBoard.Application.Administration.Queries.Accounts;
using WillBoard.Application.Administration.Queries.AccountUpdate;
using WillBoard.Application.Administration.Queries.BanAppeals;
using WillBoard.Application.Administration.Queries.BanCreate;
using WillBoard.Application.Administration.Queries.BanDelete;
using WillBoard.Application.Administration.Queries.Bans;
using WillBoard.Application.Administration.Queries.BanUpdate;
using WillBoard.Application.Administration.Queries.BoardAuthorizationDelete;
using WillBoard.Application.Administration.Queries.BoardAuthorizations;
using WillBoard.Application.Administration.Queries.BoardAuthorizationUpdate;
using WillBoard.Application.Administration.Queries.BoardBanAppeals;
using WillBoard.Application.Administration.Queries.BoardBanCreate;
using WillBoard.Application.Administration.Queries.BoardBanDelete;
using WillBoard.Application.Administration.Queries.BoardBans;
using WillBoard.Application.Administration.Queries.BoardBanUpdate;
using WillBoard.Application.Administration.Queries.BoardCreate;
using WillBoard.Application.Administration.Queries.BoardDelete;
using WillBoard.Application.Administration.Queries.BoardInvitationCreate;
using WillBoard.Application.Administration.Queries.BoardInvitations;
using WillBoard.Application.Administration.Queries.BoardIp;
using WillBoard.Application.Administration.Queries.BoardIpDeletePosts;
using WillBoard.Application.Administration.Queries.BoardPostDelete;
using WillBoard.Application.Administration.Queries.BoardPostDeleteFile;
using WillBoard.Application.Administration.Queries.BoardPostUpdate;
using WillBoard.Application.Administration.Queries.BoardReports;
using WillBoard.Application.Administration.Queries.Boards;
using WillBoard.Application.Administration.Queries.BoardThreadBumpLock;
using WillBoard.Application.Administration.Queries.BoardThreadCopy;
using WillBoard.Application.Administration.Queries.BoardThreadExcessive;
using WillBoard.Application.Administration.Queries.BoardThreadPin;
using WillBoard.Application.Administration.Queries.BoardThreadReplyLock;
using WillBoard.Application.Administration.Queries.BoardUpdate;
using WillBoard.Application.Administration.Queries.BoardViewCatalog;
using WillBoard.Application.Administration.Queries.BoardViewClassic;
using WillBoard.Application.Administration.Queries.BoardViewClassicThread;
using WillBoard.Application.Administration.Queries.BoardViewSearch;
using WillBoard.Application.Administration.Queries.Cache;
using WillBoard.Application.Administration.Queries.ConfigurationCreate;
using WillBoard.Application.Administration.Queries.ConfigurationDelete;
using WillBoard.Application.Administration.Queries.Configurations;
using WillBoard.Application.Administration.Queries.ConfigurationUpdate;
using WillBoard.Application.Administration.Queries.NavigationCreate;
using WillBoard.Application.Administration.Queries.NavigationDelete;
using WillBoard.Application.Administration.Queries.Navigations;
using WillBoard.Application.Administration.Queries.NavigationUpdate;
using WillBoard.Application.Administration.Queries.Reports;
using WillBoard.Application.Administration.Queries.TranslationCreate;
using WillBoard.Application.Administration.Queries.TranslationDelete;
using WillBoard.Application.Administration.Queries.Translations;
using WillBoard.Application.Administration.Queries.TranslationUpdate;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Web.Filters;
using WillBoard.Web.Models;
using WillBoard.Web.Services;

namespace WillBoard.Web.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly IConfigurationService _configurationService;
        private readonly IAuthenticationTokenService _authenticationTokenService;
        private readonly IMediator _mediator;
        private readonly ErrorService _errorService;

        public AdministrationController(IConfigurationService configurationService, IAuthenticationTokenService authenticationTokenService, IMediator mediator, ErrorService errorService)
        {
            _configurationService = configurationService;
            _authenticationTokenService = authenticationTokenService;
            _mediator = mediator;
            _errorService = errorService;
        }

        #region Login/Logout

        [HttpGet]
        public async Task<IActionResult> LoginAsync()
        {
            var model = new ApplicationViewModel()
            {
                Title = "Login",
                ViewType = ViewType.Application
            };

            return View("Login", model);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromForm] LoginCommand command)
        {
            var userAgentHeader = Request.Headers["User-Agent"].ToString();
            command.Name = userAgentHeader.Substring(0, Math.Min(userAgentHeader.Length, 256));

            if (_configurationService.Configuration.Administration.VerificationType == VerificationType.ReCaptcha)
            {
                command.VerificationValue = Request.Form["g-recaptcha-response"].ToString();
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.ApplicationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = result.Value.Expiration,
                HttpOnly = true,
                IsEssential = true
            };

            Response.Cookies.Append("authentication", _authenticationTokenService.Encode(result.Value.AccountId, result.Value.AuthenticationId), cookieOptions);

            return RedirectToAction("Account", new { accountId = result.Value.AccountId });
        }

        [HttpPost]
        [AuthenticationFilter]
        public async Task<IActionResult> LogoutAsync()
        {
            var command = new LogoutCommand();

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            HttpContext.Response.Cookies.Delete("authentication");

            return RedirectToAction("Login");
        }

        #endregion Login/Logout

        #region Account

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> AccountsAsync([FromRoute] int page)
        {
            var query = new AccountsQuery()
            {
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Accounts", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> AccountCreateAsync()
        {
            var query = new AccountCreateQuery();

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("AccountCreate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        public async Task<IActionResult> AccountCreateAsync([FromForm] AccountCreateCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Accounts");
        }

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> AccountAsync([FromRoute] Guid accountId)
        {
            var query = new AccountQuery()
            {
                AccountId = accountId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Account", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> AccountUpdateAsync([FromRoute] Guid accountId)
        {
            var query = new AccountUpdateQuery()
            {
                AccountId = accountId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("AccountUpdate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        public async Task<IActionResult> AccountUpdateAsync([FromRoute] Guid accountId, [FromForm] AccountUpdateCommand command)
        {
            if (accountId != command.AccountId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Account", new { accountId = accountId });
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> AccountPasswordChangeAsync([FromRoute] Guid accountId)
        {
            var query = new AccountPasswordChangeQuery()
            {
                AccountId = accountId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("AccountPasswordChange", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> AccountPasswordChangeAsync([FromRoute] Guid accountId, [FromForm] AccountPasswordChangeCommand command)
        {
            if (accountId != command.AccountId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            HttpContext.Response.Cookies.Delete("authentication");
            return RedirectToAction("Account", new { accountId = accountId });
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> AccountAuthenticationsAsync([FromRoute] Guid accountId, [FromRoute] int page)
        {
            var query = new AccountAuthenticationsQuery()
            {
                AccountId = accountId,
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("AccountAuthentications", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> AccountAuthenticationDeactivateAsync([FromRoute] Guid accountId, [FromRoute] Guid authenticationId, [FromForm] AccountAuthenticationDeactivateCommand command)
        {
            if (accountId != command.AccountId || authenticationId != command.AuthenticationId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("AccountAuthentications", new { AccountId = accountId });
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> AccountInvitationsAsync([FromRoute] Guid accountId, [FromRoute] int page)
        {
            var query = new AccountInvitationsQuery()
            {
                AccountId = accountId,
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("AccountInvitations", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> AccountInvitationAcceptAsync([FromRoute] Guid accountId, [FromRoute] Guid invitationId)
        {
            var command = new AccountInvitationAcceptCommand()
            {
                AccountId = accountId,
                InvitationId = invitationId
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("AccountInvitations", new { AccountId = accountId });
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> AccountInvitationRejectAsync([FromRoute] Guid accountId, [FromRoute] Guid invitationId)
        {
            var command = new AccountInvitationRejectCommand()
            {
                AccountId = accountId,
                InvitationId = invitationId
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("AccountInvitations", new { AccountId = accountId });
        }

        #endregion Account

        #region Board

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> BoardsAsync([FromRoute] int page)
        {
            var query = new BoardsQuery()
            {
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Boards", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardCreateAsync()
        {
            var query = new BoardCreateQuery();

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardCreate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardCreateAsync([FromForm] BoardCreateCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards", new { boardId = command.BoardId });
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardUpdateAsync([FromRoute] string boardId)
        {
            var query = new BoardUpdateQuery()
            {
                BoardId = boardId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardUpdate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardUpdateAsync([FromRoute] string boardId, [FromForm] BoardUpdateCommand command)
        {
            if (boardId != command.BoardId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardDeleteAsync([FromRoute] string boardId)
        {
            var query = new BoardDeleteQuery()
            {
                BoardId = boardId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardDelete", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardDeleteAsync([FromRoute] string boardId, [FromForm] BoardDeleteCommand command)
        {
            if (boardId != command.BoardId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        #endregion Board

        #region Board Invitation

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardInvitationsAsync([FromRoute] string boardId, [FromRoute] int page)
        {
            var query = new BoardInvitationsQuery()
            {
                BoardId = boardId,
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardInvitations", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardInvitationCreateAsync([FromRoute] string boardId)
        {
            var query = new BoardInvitationCreateQuery()
            {
                BoardId = boardId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardInvitationCreate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardInvitationCreateAsync([FromRoute] string boardId, [FromForm] BoardInvitationCreateCommand command)
        {
            if (boardId != command.BoardId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardInvitations", new { BoardId = boardId });
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardInvitationDeleteAsync([FromRoute] string boardId, [FromRoute] Guid invitationId, [FromForm] BoardInvitationDeleteCommand command)
        {
            if (boardId != command.BoardId || invitationId != command.InvitationId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardInvitations", new { BoardId = boardId });
        }

        #endregion Board Invitation

        #region Board Authorization

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardAuthorizationsAsync([FromRoute] string boardId, [FromRoute] int page)
        {
            var query = new BoardAuthorizationsQuery()
            {
                BoardId = boardId,
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardAuthorizations", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardAuthorizationUpdateAsync([FromRoute] string boardId, [FromRoute] Guid authorizationId)
        {
            var query = new BoardAuthorizationUpdateQuery()
            {
                BoardId = boardId,
                AuthorizationId = authorizationId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardAuthorizationUpdate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardAuthorizationUpdateAsync([FromRoute] string boardId, [FromRoute] Guid authorizationId, [FromForm] BoardAuthorizationUpdateCommand command)
        {
            if (boardId != command.BoardId || authorizationId != command.AuthorizationId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardAuthorizations", new { BoardId = boardId });
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardAuthorizationDeleteAsync([FromRoute] string boardId, [FromRoute] Guid authorizationId)
        {
            var query = new BoardAuthorizationDeleteQuery()
            {
                BoardId = boardId,
                AuthorizationId = authorizationId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardAuthorizationDelete", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardAuthorizationDeleteAsync([FromRoute] string boardId, [FromRoute] Guid authorizationId, [FromForm] BoardAuthorizationDeleteCommand command)
        {
            if (boardId != command.BoardId || authorizationId != command.AuthorizationId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardAuthorizations", new { BoardId = boardId });
        }

        #endregion Board Authorization

        #region Board Ban

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> BoardBansAsync([FromRoute] string boardId, [FromRoute] int page)
        {
            var query = new BoardBansQuery()
            {
                BoardId = boardId,
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardBans", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardBanCreateAsync([FromRoute] string boardId, [FromQuery] IpVersion ipVersion = IpVersion.None, [FromQuery] string ipNumber = "")
        {
            var query = new BoardBanCreateQuery()
            {
                BoardId = boardId,
                IPVersion = ipVersion,
                IPNumber = ipNumber
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardBanCreate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardBanCreateAsync([FromRoute] string boardId, [FromForm] BoardBanCreateCommand command)
        {
            if (boardId != command.BoardId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardBans", new { BoardId = boardId });
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardBanUpdateAsync([FromRoute] string boardId, [FromRoute] Guid banId)
        {
            var query = new BoardBanUpdateQuery()
            {
                BoardId = boardId,
                BanId = banId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardBanUpdate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardBanUpdateAsync([FromRoute] string boardId, [FromRoute] Guid banId, [FromForm] BoardBanUpdateCommand command)
        {
            if (boardId != command.BoardId || banId != command.BanId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardBans", new { BoardId = boardId });
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardBanDeleteAsync([FromRoute] string boardId, [FromRoute] Guid banId)
        {
            var query = new BoardBanDeleteQuery()
            {
                BoardId = boardId,
                BanId = banId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardBanDelete", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardBanDeleteAsync([FromRoute] string boardId, [FromRoute] Guid banId, [FromForm] BoardBanDeleteCommand command)
        {
            if (boardId != command.BoardId || banId != command.BanId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardBans", new { BoardId = boardId });
        }

        #endregion Board Ban

        #region Board Ban Appeal

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardBanAppealsAsync([FromRoute] string boardId, [FromRoute] int page)
        {
            var query = new BoardBanAppealsQuery()
            {
                BoardId = boardId,
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardBanAppeals", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardBanAppealAcceptAsync([FromRoute] string boardId, [FromRoute] Guid banAppealId, [FromForm] BoardBanAppealAcceptCommand command)
        {
            if (boardId != command.BoardId || banAppealId != command.BanAppealId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardBanAppeals", new { boardId = boardId });
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardBanAppealRejectAsync([FromRoute] string boardId, [FromRoute] Guid banAppealId, [FromForm] BoardBanAppealRejectCommand command)
        {
            if (boardId != command.BoardId || banAppealId != command.BanAppealId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardBanAppeals", new { boardId = boardId });
        }

        #endregion Board Ban Appeal

        #region Board Report

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardReportsAsync([FromRoute] string boardId, [FromRoute] int page)
        {
            var query = new BoardReportsQuery()
            {
                BoardId = boardId,
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardReports", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardReportDeleteAsync([FromRoute] string boardId, [FromRoute] Guid reportId, [FromForm] BoardReportDeleteCommand command)
        {
            if (boardId != command.BoardId || reportId != command.ReportId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BoardReports", new { boardId = boardId });
        }

        #endregion Board Report

        #region Board Post

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardPostUpdateAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new BoardPostUpdateQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardPostUpdate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardPostUpdateAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] BoardPostUpdateCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardPostDeleteAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new BoardPostDeleteQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardPostDelete", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardPostDeleteAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] BoardPostDeleteCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardPostDeleteFileAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new BoardPostDeleteFileQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardPostDeleteFile", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardPostDeleteFileAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] BoardPostDeleteFileCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardThreadReplyLockAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new BoardThreadReplyLockQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardThreadReplyLock", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardThreadReplyLockAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] BoardThreadReplyLockCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardThreadBumpLockAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new BoardThreadBumpLockQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardThreadBumpLock", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardThreadBumpLockAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] BoardThreadBumpLockCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardThreadExcessiveAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new BoardThreadExcessiveQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardThreadExcessive", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardThreadExcessiveAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] BoardThreadExcessiveCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardThreadPinAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new BoardThreadPinQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardThreadPin", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardThreadPinAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] BoardThreadPinCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardThreadCopyAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new BoardThreadCopyQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardThreadCopy", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardThreadCopyAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] BoardThreadCopyCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        #endregion Board Post

        #region Board IP

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> BoardIpAsync([FromRoute] string boardId, [FromRoute] IpVersion ipVersion, [FromRoute] string ipNumber)
        {
            var query = new BoardIpQuery()
            {
                BoardId = boardId,
                IpVersion = ipVersion,
                IpNumber = ipNumber
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardIp", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardIpDeletePostsAsync([FromRoute] string boardId, [FromRoute] IpVersion ipVersion, [FromRoute] string ipNumber)
        {
            var query = new BoardIpDeletePostsQuery()
            {
                BoardId = boardId,
                IpVersion = ipVersion,
                IpNumber = ipNumber
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardIpDeletePosts", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BoardIpDeletePostsAsync([FromRoute] string boardId, [FromRoute] IpVersion ipVersion, [FromRoute] string ipNumber, [FromForm] BoardIpDeletePostsCommand command)
        {
            if (boardId != command.BoardId || ipVersion != command.IpVersion || ipNumber != command.IpNumber)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Boards");
        }

        #endregion Board IP

        #region Bans

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> BansAsync([FromRoute] int page)
        {
            var query = new BansQuery()
            {
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Bans", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BanCreateAsync([FromQuery] IpVersion ipVersion = IpVersion.None, [FromQuery] string ipNumber = "")
        {
            var query = new BanCreateQuery()
            {
                IPVersion = ipVersion,
                IPNumber = ipNumber
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BanCreate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BanCreateAsync([FromForm] BanCreateCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Bans");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BanUpdateAsync([FromRoute] Guid banId)
        {
            var query = new BanUpdateQuery()
            {
                BanId = banId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BanUpdate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BanUpdateAsync([FromRoute] Guid banId, [FromForm] BanUpdateCommand command)
        {
            if (banId != command.BanId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Bans");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BanDeleteAsync([FromRoute] Guid banId)
        {
            var query = new BanDeleteQuery()
            {
                BanId = banId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BanDelete", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BanDeleteAsync([FromRoute] Guid banId, [FromForm] BanDeleteCommand command)
        {
            if (banId != command.BanId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Bans");
        }

        #endregion Bans

        #region Ban Appeal

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BanAppealsAsync([FromRoute] int page)
        {
            var query = new BanAppealsQuery()
            {
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BanAppeals", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BanAppealAcceptAsync([FromRoute] Guid banAppealId, [FromForm] BanAppealAcceptCommand command)
        {
            if (banAppealId != command.BanAppealId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BanAppeals");
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BanAppealRejectAsync([FromRoute] Guid banAppealId, [FromForm] BanAppealRejectCommand command)
        {
            if (banAppealId != command.BanAppealId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("BanAppeals");
        }

        #endregion Ban Appeal

        #region Report

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> ReportsAsync([FromRoute] int page)
        {
            var query = new ReportsQuery()
            {
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Reports", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> ReportDeleteAsync([FromRoute] Guid reportId, [FromForm] ReportDeleteCommand command)
        {
            if (reportId != command.ReportId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Reports");
        }

        #endregion Report

        #region Navigation

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> NavigationsAsync()
        {
            var query = new NavigationsQuery();

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Navigations", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> NavigationCreateAsync()
        {
            var query = new NavigationCreateQuery();

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("NavigationCreate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> NavigationCreateAsync([FromForm] NavigationCreateCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Navigations");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> NavigationUpdateAsync([FromRoute] Guid navigationId)
        {
            var query = new NavigationUpdateQuery()
            {
                NavigationId = navigationId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("NavigationUpdate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> NavigationUpdateAsync([FromRoute] Guid navigationId, [FromForm] NavigationUpdateCommand command)
        {
            if (navigationId != command.NavigationId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Navigations");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> NavigationDeleteAsync([FromRoute] Guid navigationId)
        {
            var query = new NavigationDeleteQuery()
            {
                NavigationId = navigationId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("NavigationDelete", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> NavigationDeleteAsync([FromRoute] Guid navigationId, [FromForm] NavigationDeleteCommand command)
        {
            if (navigationId != command.NavigationId)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Navigations");
        }

        #endregion Navigation

        #region Configuration

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> ConfigurationsAsync()
        {
            var query = new ConfigurationsQuery();

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Configurations", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> ConfigurationCreateAsync()
        {
            var query = new ConfigurationCreateQuery();

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("ConfigurationCreate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> ConfigurationCreateAsync([FromForm] ConfigurationCreateCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Configurations");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> ConfigurationUpdateAsync([FromRoute] string key)
        {
            var query = new ConfigurationUpdateQuery()
            {
                Key = key
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("ConfigurationUpdate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> ConfigurationUpdateAsync([FromRoute] string key, [FromForm] ConfigurationUpdateCommand command)
        {
            if (key != command.Key)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Configurations");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> ConfigurationDeleteAsync([FromRoute] string key)
        {
            var query = new ConfigurationDeleteQuery()
            {
                Key = key
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("ConfigurationDelete", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> ConfigurationDeleteAsync([FromRoute] string key, [FromForm] ConfigurationDeleteCommand command)
        {
            if (key != command.Key)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Configurations");
        }

        #endregion Configuration

        #region Translation

        [HttpGet]
        [AuthenticationFilter]
        public async Task<IActionResult> TranslationsAsync()
        {
            var query = new TranslationsQuery();

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Translations", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> TranslationCreateAsync()
        {
            var query = new TranslationCreateQuery();

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("TranslationCreate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> TranslationCreateAsync([FromForm] TranslationCreateCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Translations");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> TranslationUpdateAsync([FromRoute] string language, [FromRoute] string key)
        {
            var query = new TranslationUpdateQuery()
            {
                Language = language,
                Key = key
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("TranslationUpdate", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> TranslationUpdateAsync([FromRoute] string language, [FromRoute] string key, [FromForm] TranslationUpdateCommand command)
        {
            if (language != command.Language || key != command.Key)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Translations");
        }

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> TranslationDeleteAsync([FromRoute] string language, [FromRoute] string key)
        {
            var query = new TranslationDeleteQuery()
            {
                Language = language,
                Key = key
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("TranslationDelete", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> TranslationDeleteAsync([FromRoute] string language, [FromRoute] string key, [FromForm] TranslationDeleteCommand command)
        {
            if (language != command.Language || key != command.Key)
            {
                return await _errorService.AdministrationErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Translations");
        }

        #endregion Translation

        #region Cache

        [HttpGet]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> CacheAsync()
        {
            var query = new CacheQuery()
            {
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Cache", result.Value);
        }

        [HttpPost]
        [AuthenticationFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> CacheClearAsync([FromForm] CacheClearCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.AdministrationErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Cache");
        }

        #endregion Cache

        #region Board View

        [HttpGet]
        [AuthenticationFilter]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardViewClassicAsync([FromRoute] string boardId, [FromRoute] int page = 1)
        {
            var query = new BoardViewClassicQuery()
            {
                BoardId = boardId,
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardViewClassic", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardViewClassicThreadAsync([FromRoute] string boardId, [FromRoute] int threadId, [FromRoute] int last)
        {
            var query = new BoardViewClassicThreadQuery()
            {
                BoardId = boardId,
                ThreadId = threadId,
                Last = last
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardViewClassicThread", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardViewCatalogAsync([FromRoute] string boardId)
        {
            var query = new BoardViewCatalogQuery()
            {
                BoardId = boardId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardViewCatalog", result.Value);
        }

        [HttpGet]
        [AuthenticationFilter]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BoardViewSearchAsync([FromRoute] string boardId, [FromQuery] string message, [FromQuery] int? postId, [FromQuery] int? threadId, [FromQuery] string file, [FromQuery] string type, [FromQuery] string order)
        {
            var query = new BoardViewSearchQuery()
            {
                BoardId = boardId,
                Message = message,
                PostId = postId,
                ThreadId = threadId,
                File = file,
                Type = type,
                Order = order,
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("BoardViewSearch", result.Value);
        }

        #endregion Board View
    }
}