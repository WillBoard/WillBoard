using WillBoard.Core.Entities;

namespace WillBoard.Application.ViewModels
{
    public class ReplyViewModel : BoardViewModel
    {
        public Post Thread { get; set; }
        public Post Reply { get; set; }
    }
}