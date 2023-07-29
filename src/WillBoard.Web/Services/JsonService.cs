using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WillBoard.Core.Converters;
using WillBoard.Core.Enums;
using WillBoard.Web.Models.Json;

namespace WillBoard.Infrastructure.Services
{
    public class JsonService
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new DateTimeJsonConverter(),
                new BigIntegerJsonConverter()
            }
        };

        public string SerializeData<T>(T data)
        {
            var json = new Json()
            {
                Data = data
            };

            return JsonSerializer.Serialize(json, _jsonSerializerOptions);
        }

        public string SerializeError<T>(T error)
        {
            var json = new Json()
            {
                Error = error
            };

            return JsonSerializer.Serialize(json, _jsonSerializerOptions);
        }

        public string SerializeSynchronizationMessage<T>(SynchronizationEvent synchronizationEvent, T synchronizationData)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("event:");
            stringBuilder.AppendLine(synchronizationEvent.ToString().ToLowerInvariant());
            stringBuilder.Append("data:");

            var jsonData = JsonSerializer.Serialize(synchronizationData, _jsonSerializerOptions);
            stringBuilder.AppendLine(jsonData);

            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }
    }
}