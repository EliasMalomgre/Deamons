using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace UI_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            /*var configuration = new ConfigurationBuilder()
        .AddCommandLine(args)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var hostUrl = configuration["hosturl"];
    if (string.IsNullOrEmpty(hostUrl)) hostUrl = "https://0.0.0.0:5001";

    var host = new WebHostBuilder()
        .UseKestrel()
        .UseUrls(hostUrl)   // <!-- this
        .UseStartup<Startup>()
        .UseConfiguration(configuration)
        .Build();

    host.Run();*/
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions => { serverOptions.Listen(IPAddress.Any, 5000); });
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}