using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using WillBoard.Core.Classes;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services.Instance
{
    public class InstanceConfigurationService : IConfigurationService
    {
        private readonly IHostEnvironment _hostEnvironment;

        public InstanceConfigurationService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;

            SetConfiguration();
        }

        public Configuration Configuration { get; set; }

        public void SetConfiguration()
        {
            Configuration = ReadConfiguration(_hostEnvironment.ContentRootPath);
        }

        private Configuration ReadConfiguration(string contentRootPath)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true
            };

            options.Converters.Add(new JsonStringEnumConverter());

            var path = contentRootPath + "/configuration.json";

            if (!File.Exists(path))
            {
                throw new Exception("Could not find instance configuration file (configuration.json).");
            }

            var configuration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(path), options);

            if (configuration is null)
            {
                throw new Exception("Deserialized instance configuration file is null.");
            }

            return configuration;
        }
    }
}