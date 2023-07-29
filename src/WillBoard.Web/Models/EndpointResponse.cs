namespace WillBoard.Web.Models
{
    public class EndpointResponse
    {
        public EndpointContentType ContentType { get; private set; }

        public EndpointResponse(EndpointContentType contentType)
        {
            ContentType = contentType;
        }
    }

    public enum EndpointContentType
    {
        HTML = 0,
        JSON = 1,
        SSE = 2
    }
}