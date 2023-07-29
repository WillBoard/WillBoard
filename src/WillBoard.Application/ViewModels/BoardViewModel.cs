using WillBoard.Core.Enums;

namespace WillBoard.Application.ViewModels
{
    public class BoardViewModel : ApplicationViewModel
    {
        public BoardViewType BoardViewType { get; set; }
        public bool Verification { get; set; }
    }
}