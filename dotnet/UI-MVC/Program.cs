using System.Net;
using BL.Domain.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UI.MVC.Services;

namespace UI.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    IdentityDataInitializer.SeedData
                        (userManager, roleManager);
                }
                catch
                {
                }

                host.Run();
            }

            /*var configuration = new ConfigurationBuilder()
                .AddCommandLine(args)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var hostUrl = configuration["hosturl"];
            if (string.IsNullOrEmpty(hostUrl)) hostUrl = "http://0.0.0.0:5001";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(hostUrl)   // <!-- this
                .UseStartup<Startup>()
                .UseConfiguration(configuration)
                .Build();

            host.Run();*/
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions => { serverOptions.Listen(IPAddress.Any, 5001); });
                    webBuilder.UseStartup<Startup>();
                });
    }
}