namespace WillBoard.Application.Board.Commands.CreatePost
{
    public class CreatePostDataModel
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public int? ThreadId { get; set; }
    }
}