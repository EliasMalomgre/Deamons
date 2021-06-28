using Microsoft.AspNetCore.Hosting;
using UI.MVC.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace UI.MVC.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}