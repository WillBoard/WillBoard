using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services
{
    public class ReCaptchaV2Service : IReCaptchaV2Service
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ReCaptchaV2Service(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> VerifyAsync(string secret, string value)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={value}");
            var deserializeResponse = JsonSerializer.Deserialize<ReCaptchaV2Response>(response, _jsonSerializerOptions);
            return deserializeResponse.Success;
        }

        public class ReCaptchaV2Response
        {
            public bool Success { get; set; }
            public List<string> ErrorCodes { get; set; }
        }
    }
}