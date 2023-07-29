using WillBoard.Core.Entities;

namespace WillBoard.Application.ViewModels
{
    public class ThreadViewModel : BoardViewModel
    {
        public Post Thread { get; set; }
        public Post[] ReplyCollection { get; set; }
    }
}