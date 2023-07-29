using WillBoard.Core.Errors;

namespace WillBoard.Application.ViewModels
{
    public class BoardErrorViewModel : BoardViewModel
    {
        public ExternalError Error { get; set; }
    }
}