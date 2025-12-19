using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services
{
    public class TurnstileService : ITurnstileService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public TurnstileService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> VerifyAsync(string secret, string value)
        {
            var client = _httpClientFactory.CreateClient();

            var parameters = new Dictionary<string, string>
            {
                { "secret", secret },
                { "response", value }
            };
            var postContent = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", postContent);
            var stringContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(stringContent))
            {
                return false;
            }

            var deserializeResponse = JsonSerializer.Deserialize<TurnstileResponse>(stringContent, _jsonSerializerOptions);

            if (deserializeResponse is null)
            {
                return false;
            }

            return deserializeResponse.Success;
        }

        public class TurnstileResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }
    }
}
