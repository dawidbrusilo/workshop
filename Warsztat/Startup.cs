using Microsoft.Owin;
using Owin;
using TrackerEnabledDbContext.Common.Configuration;

[assembly: OwinStartupAttribute(typeof(Warsztat.Startup))]
namespace Warsztat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalTrackingConfig.DisconnectedContext = true;
        }
    }
}
