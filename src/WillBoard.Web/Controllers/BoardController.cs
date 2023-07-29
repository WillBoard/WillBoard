using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using WillBoard.Application.Board.Commands.BanAppealBoard;
using WillBoard.Application.Board.Commands.BanAppealSystem;
using WillBoard.Application.Board.Commands.CreatePost;
using WillBoard.Application.Board.Commands.DeleteFile;
using WillBoard.Application.Board.Commands.DeletePost;
using WillBoard.Application.Board.Commands.ReportBoard;
using WillBoard.Application.Board.Commands.ReportSystem;
using WillBoard.Application.Board.Queries.Ban;
using WillBoard.Application.Board.Queries.Catalog;
using WillBoard.Application.Board.Queries.Classic;
using WillBoard.Application.Board.Queries.ClassicThread;
using WillBoard.Application.Board.Queries.Delete;
using WillBoard.Application.Board.Queries.Report;
using WillBoard.Application.Board.Queries.Search;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Extensions;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Web.Filters;
using WillBoard.Web.Models;
using WillBoard.Web.Services;

namespace WillBoard.Web.Controllers
{
    [OnlineCounterFilter]
    public class BoardController : Controller
    {
        private readonly IMediator _mediator;
        private readonly BoardManager _boardManager;
        private readonly IBoardCache _boardCahe;
        private readonly LinkGenerator _linkGenerator;
        private readonly ErrorService _errorService;

        public BoardController(IMediator mediator, BoardManager boardManager, IBoardCache boardCahe, LinkGenerator linkGenerator, ErrorService errorService)
        {
            _mediator = mediator;
            _boardManager = boardManager;
            _boardCahe = boardCahe;
            _errorService = errorService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> ClassicAsync([FromRoute] string boardId, [FromRoute] int page = 1)
        {
            var query = new ClassicQuery()
            {
                BoardId = boardId,
                Page = page
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Classic", result.Value);
        }

        [HttpGet]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> ClassicThreadAsync([FromRoute] string boardId, [FromRoute] int threadId, [FromRoute] int? last)
        {
            var query = new ClassicThreadQuery()
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

            return View("ClassicThread", result.Value);
        }

        [HttpGet]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> CatalogAsync([FromRoute] string boardId)
        {
            var query = new CatalogQuery()
            {
                BoardId = boardId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Catalog", result.Value);
        }

        [HttpGet]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> SearchAsync([FromRoute] string boardId, [FromQuery] string message, [FromQuery] int? postId, [FromQuery] int? threadId, [FromQuery] string file, [FromQuery] string type, [FromQuery] string order)
        {
            var query = new SearchQuery()
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

            return View("Search", result.Value);
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> CreatePostAsync([FromRoute] string boardId, [FromForm] CreatePostCommand command)
        {
            if (boardId != command.BoardId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var userAgentHeader = Request.Headers["User-Agent"].ToString();
            command.UserAgent = userAgentHeader.Substring(0, Math.Min(userAgentHeader.Length, 256));

            var board = _boardManager.GetBoard();

            if (board.FieldVerificationType == VerificationType.ReCaptcha)
            {
                command.VerificationValue = Request.Form["g-recaptcha-response"].ToString();
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            var link = _linkGenerator.GetPathToClassicThread(result.Value.BoardId, result.Value.ThreadId, result.Value.PostId, AnchorType.Default);

            return Redirect(link);
        }

        [HttpGet]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new DeleteQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Delete", result.Value);
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> DeletePostAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] DeletePostCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Classic", "Board", new { boardId = boardId });
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> DeleteFileAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] DeleteFileCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Classic", "Board", new { boardId = boardId });
        }

        [HttpGet]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> ReportAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new ReportQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Report", result.Value);
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> ReportBoardAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] ReportBoardCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Classic", "Board", new { boardId = boardId });
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> ReportSystemAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] ReportSystemCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Classic", "Board", new { boardId = boardId });
        }

        [HttpGet]
        [BoardFilter]
        [CsrfFilter(Csrf.Set)]
        public async Task<IActionResult> BanAsync([FromRoute] string boardId)
        {
            var query = new BanQuery()
            {
                BoardId = boardId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return View("Ban", result.Value);
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BanAppealBoardAsync([FromRoute] string boardId, [FromRoute] Guid banId, [FromForm] BanAppealBoardCommand command)
        {
            if (boardId != command.BoardId || banId != command.BanId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Ban", "Board", new { boardId = boardId });
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> BanAppealSystemAsync([FromRoute] string boardId, [FromRoute] Guid banId, [FromForm] BanAppealSystemCommand command)
        {
            if (boardId != command.BoardId || banId != command.BanId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.HTML);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.HTML);
            }

            return RedirectToAction("Ban", "Board", new { boardId = boardId });
        }

        [HttpPost]
        public async Task<IActionResult> SecurePassword([FromRoute] string boardId, [FromForm] string password)
        {
            var board = await _boardCahe.GetAsync(boardId);

            if (board == null || board.Availability == false)
            {
                return await _errorService.ApplicationErrorAsync(new InternalError(404, "NotFound"), EndpointContentType.HTML);
            }

            if (board.Accessibility == BoardAccessibility.Password)
            {
                if (!HttpContext.Request.Cookies.TryGetValue("password_" + board.BoardId.ToLower(), out string cookiePasswordValue))
                {
                    if (board.AccessibilityPassword != cookiePasswordValue)
                    {
                        if (password == board.AccessibilityPassword)
                        {
                            HttpContext.Response.Cookies.Append("password_" + board.BoardId.ToLower(), board.AccessibilityPassword);
                        }
                    }
                }
            }

            return RedirectToAction("Classic", "Board", new { boardId = board.BoardId });
        }
    }
}