using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace WillBoard.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.UseSystemd();
                    });
                    webHostBuilder.UseWebRoot("wwwroot");
                    webHostBuilder.UseStartup<Startup>();
                });

            hostBuilder.Build().Run();
        }
    }
}