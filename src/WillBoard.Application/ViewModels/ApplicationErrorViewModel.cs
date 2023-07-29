using WillBoard.Core.Errors;

namespace WillBoard.Application.ViewModels
{
    public class ApplicationErrorViewModel : ApplicationViewModel
    {
        public ExternalError Error { get; set; }
    }
}