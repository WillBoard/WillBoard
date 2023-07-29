namespace WillBoard.Core.Entities
{
    public class PostMention
    {
        public string OutcomingBoardId { get; set; }
        public int OutcomingPostId { get; set; }
        public int? OutcomingThreadId { get; set; }

        public string IncomingBoardId { get; set; }
        public int IncomingPostId { get; set; }
        public int? IncomingThreadId { get; set; }

        public bool Active { get; set; }

        public PostMention()
        {
        }
    }
}