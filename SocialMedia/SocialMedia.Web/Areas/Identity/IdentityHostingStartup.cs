using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(SocialMedia.Web.Areas.Identity.IdentityHostingStartup))]
namespace SocialMedia.Web.Areas.Identity
{

    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}