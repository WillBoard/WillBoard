using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Classes;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Infrastructure.Services;
using WillBoard.Web.Models;

namespace WillBoard.Web.Services
{
    public class ErrorService
    {
        private readonly ILocalizationService _localizationService;
        private readonly BoardManager _boardManager;
        private readonly AccountManager _accountManager;
        private readonly JsonService _jsonService;

        public ErrorService(ILocalizationService localizationService, BoardManager boardManager, AccountManager accountManager, JsonService jsonService)
        {
            _localizationService = localizationService;
            _boardManager = boardManager;
            _accountManager = accountManager;
            _jsonService = jsonService;
        }

        public async Task<IActionResult> ApplicationErrorAsync(InternalError internalError, EndpointContentType endpointContentType, string language = "en")
        {
            var message = await _localizationService.GetLocalizationAsync(language, internalError.Code, internalError.Arguments);
            var externalError = new ExternalError(internalError.Status, internalError.Code, message);
            return await ErrorAsync(externalError, endpointContentType, ErrorType.Application);
        }

        public async Task<IActionResult> BoardErrorAsync(InternalError internalError, EndpointContentType endpointContentType, string language = "en")
        {
            var message = await _localizationService.GetLocalizationAsync(language, internalError.Code, internalError.Arguments);
            var externalError = new ExternalError(internalError.Status, internalError.Code, message);
            return await ErrorAsync(externalError, endpointContentType, ErrorType.Board);
        }

        public async Task<IActionResult> AdministrationErrorAsync(InternalError internalError, EndpointContentType endpointContentType, string language = "en")
        {
            var message = await _localizationService.GetLocalizationAsync(language, internalError.Code, internalError.Arguments);
            var externalError = new ExternalError(internalError.Status, internalError.Code, message);
            return await ErrorAsync(externalError, endpointContentType, ErrorType.Administration);
        }

        private async Task<IActionResult> ErrorAsync(ExternalError externalError, EndpointContentType endpointContentType, ErrorType errorType)
        {
            if (endpointContentType == EndpointContentType.JSON)
            {
                return new ContentResult()
                {
                    StatusCode = externalError.Status,
                    ContentType = ContentType.ApplicationJson,
                    Content = _jsonService.SerializeError(externalError)
                };
            }
            else
            {
                if (errorType == ErrorType.Board)
                {
                    return GenerateBoardErrorViewResult(externalError);
                }

                if (errorType == ErrorType.Administration)
                {
                    return GenerateAdministrationErrorViewResult(externalError);
                }

                return GenerateApplicationErrorViewResult(externalError);
            }
        }

        private ViewResult GenerateApplicationErrorViewResult(ExternalError externalError)
        {
            var model = new ApplicationErrorViewModel()
            {
                Title = "Error",
                ViewType = ViewType.Application,
                Error = externalError
            };

            return new ViewResult
            {
                StatusCode = externalError.Status,
                ViewName = "/Views/Application/Error.cshtml",
                ViewData = new ViewDataDictionary(metadataProvider: new EmptyModelMetadataProvider(), modelState: new ModelStateDictionary())
                {
                    Model = model
                },
                TempData = new MemoryTempDataDictionary()
            };
        }

        private ViewResult GenerateBoardErrorViewResult(ExternalError externalError)
        {
            var board = _boardManager.GetBoard();

            if (board is null)
            {
                return GenerateApplicationErrorViewResult(externalError);
            }

            var model = new BoardErrorViewModel()
            {
                Title = $"/{board.BoardId}/ - {board.Name}",
                Error = externalError
            };

            return new ViewResult
            {
                StatusCode = externalError.Status,
                ViewName = "/Views/Board/Error.cshtml",
                ViewData = new ViewDataDictionary(metadataProvider: new EmptyModelMetadataProvider(), modelState: new ModelStateDictionary())
                {
                    Model = model
                },
                TempData = new MemoryTempDataDictionary()
            };
        }

        private ViewResult GenerateAdministrationErrorViewResult(ExternalError externalError)
        {
            var account = _accountManager.GetAccount();

            if (account is null)
            {
                return GenerateApplicationErrorViewResult(externalError);
            }

            var model = new AdministrationErrorViewModel()
            {
                Title = "Error",
                Error = externalError
            };

            return new ViewResult
            {
                StatusCode = externalError.Status,
                ViewName = "/Views/Administration/Error.cshtml",
                ViewData = new ViewDataDictionary(metadataProvider: new EmptyModelMetadataProvider(), modelState: new ModelStateDictionary())
                {
                    Model = model
                },
                TempData = new MemoryTempDataDictionary()
            };
        }

        public enum ErrorType
        {
            Application = 0,
            Board = 1,
            Administration = 2
        }
    }
}