using System;
using System.IO;
using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
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

                var webApplicationOptions = new WebApplicationOptions()
                {
                    Args = args,
                    ContentRootPath = Directory.GetCurrentDirectory(),
                    WebRootPath = "wwwroot"
                };
                var webApplicationBuilder = WebApplication.CreateEmptyBuilder(webApplicationOptions);

                webApplicationBuilder.WebHost.UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.UseSystemd();
                });

                var _configurationService = new InstanceConfigurationService(webApplicationBuilder.Environment);

                webApplicationBuilder.Services.AddSingleton<IConfigurationService>(_configurationService);

                webApplicationBuilder.Services.AddLogging(builder =>
                {
                    builder.ClearProviders();

                    builder.AddJsonConsole(options =>
                    {
                        options.IncludeScopes = false;
                        options.UseUtcTimestamp = true;
                        options.JsonWriterOptions = new JsonWriterOptions
                        {
                            Indented = true,
                        };
                    });

                    builder.SetMinimumLevel(_configurationService.Configuration.Logger.Level);

                    foreach (var filter in _configurationService.Configuration.Logger.FilterCollection)
                    {
                        builder.AddFilter(filter.Key, filter.Value);
                    }
                });

                webApplicationBuilder.Services.AddTransient<MarkupService>();

                webApplicationBuilder.Services.AddScoped<IpManager>();
                webApplicationBuilder.Services.AddScoped<AccountManager>();
                webApplicationBuilder.Services.AddScoped<BoardManager>();

                webApplicationBuilder.Services.AddInfrastructure(_configurationService);

                webApplicationBuilder.Services.AddApplication();

                webApplicationBuilder.Services.AddMvcCore(options => options.ModelMetadataDetailsProviders.Add(new StringMetadataProvider()))
                    .AddRazorViewEngine();

                // For render ViewResult from middleware
                // eg. context.RequestServices.GetService<IActionResultExecutor<ViewResult>>();
                webApplicationBuilder.Services.AddSingleton<IActionResultExecutor<ViewResult>, ViewResultExecutor>();

                webApplicationBuilder.Services.AddSingleton<JsonService>();

                webApplicationBuilder.Services.AddScoped<ViewService>();

                webApplicationBuilder.Services.AddScoped<ErrorService>();

                webApplicationBuilder.Services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                });

                var webApplication = webApplicationBuilder.Build();

                webApplication.UseForwardedHeaders();

                if (webApplication.Environment.IsDevelopment())
                {
                    webApplication.UseDeveloperExceptionPage();
                    webApplication.UseStatusCodePages();
                }
                else
                {
                    webApplication.UseMiddleware<ExceptionHandlerMiddleware>();
                    webApplication.UseMiddleware<StatusCodeHandlerMiddleware>();
                }

                var contentTypeProvider = new FileExtensionContentTypeProvider();
                contentTypeProvider.Mappings[".flac"] = "audio/flac";

                webApplication.UseStaticFiles(new StaticFileOptions
                {
                    ContentTypeProvider = contentTypeProvider
                });

                webApplication.UseMiddleware<IpNumberMiddleware>();

                Routing.Register(webApplication);

                webApplication.Run();
            }
            catch (Exception exception)
            {
                throw new Exception("Host terminated unexpectedly.", exception);
            }
        }
    }
}
