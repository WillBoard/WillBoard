using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using WillBoard.Application;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Services;
using WillBoard.Infrastructure;
using WillBoard.Infrastructure.Services;
using WillBoard.Infrastructure.Services.Instance;
using WillBoard.Infrastructure.TypeHandlers;
using WillBoard.Web.Middlewares;
using WillBoard.Web.Providers;
using WillBoard.Web.Services;

namespace WillBoard.Web
{
    public class Startup
    {
        private readonly IConfigurationService _configurationService;

        public Startup(IHostEnvironment hostEnvironment)
        {
            _configurationService = new InstanceConfigurationService(hostEnvironment);

            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            SqlMapper.AddTypeHandler(new UInt128TypeHandler());
            SqlMapper.AddTypeHandler(new StringArrayTypeHandler());
            SqlMapper.AddTypeHandler(new UInt32ArrayTypeHandler());
            SqlMapper.AddTypeHandler(new UInt128ArrayTypeHandler());
            SqlMapper.AddTypeHandler(new CssThemeArrayTypeHandler());
            SqlMapper.AddTypeHandler(new MarkupCustomArrayTypeHandler());
            SqlMapper.AddTypeHandler(new BlockListArrayTypeHandler());
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConfigurationService>(_configurationService);

            serviceCollection.AddLogging(builder =>
            {
                builder.ClearProviders();

                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = false;
                    options.SingleLine = false;
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                    options.UseUtcTimestamp = true;
                });

                builder.SetMinimumLevel(_configurationService.Configuration.Logger.Level);

                foreach (var filter in _configurationService.Configuration.Logger.FilterCollection)
                {
                    builder.AddFilter(filter.Key, filter.Value);
                }
            });

            serviceCollection.AddTransient<MarkupService>();

            serviceCollection.AddScoped<IpManager>();
            serviceCollection.AddScoped<AccountManager>();
            serviceCollection.AddScoped<BoardManager>();

            serviceCollection.AddInfrastructure(_configurationService);

            serviceCollection.AddApplication();

            serviceCollection.AddMvcCore(options => options.ModelMetadataDetailsProviders.Add(new StringMetadataProvider()))
                .AddRazorViewEngine()
                .AddRazorRuntimeCompilation();

            // For render ViewResult from middleware
            // eg. context.RequestServices.GetService<IActionResultExecutor<ViewResult>>();
            serviceCollection.AddSingleton<IActionResultExecutor<ViewResult>, ViewResultExecutor>();

            serviceCollection.AddSingleton<JsonService>();

            serviceCollection.AddScoped<ViewService>();

            serviceCollection.AddScoped<ErrorService>();

            serviceCollection.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment hostEnvironment)
        {
            applicationBuilder.UseForwardedHeaders();

            if (hostEnvironment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
                applicationBuilder.UseStatusCodePages();
            }
            else
            {
                applicationBuilder.UseMiddleware<ExceptionHandlerMiddleware>();
                applicationBuilder.UseMiddleware<StatusCodeHandlerMiddleware>();
            }

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".flac"] = "audio/flac";

            applicationBuilder.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            applicationBuilder.UseMiddleware<IpNumberMiddleware>();

            applicationBuilder.UseRouting();

            applicationBuilder.UseEndpoints(endpoints =>
            {
                Routing.Generate(endpoints);
            });
        }
    }
}