using WillBoard.Core.Errors;

namespace WillBoard.Application.ViewModels
{
    public class AdministrationErrorViewModel : AdministrationViewModel
    {
        public ExternalError Error { get; set; }
    }
}