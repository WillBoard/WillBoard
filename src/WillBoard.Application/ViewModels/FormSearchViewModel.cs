namespace WillBoard.Application.ViewModels
{
    public class FormSearchViewModel : BoardViewModel
    {
        public int? PostId { get; set; }
        public int? ThreadId { get; set; }
        public string Message { get; set; }
        public string File { get; set; }
        public string Type { get; set; }
        public string Order { get; set; }
    }
}