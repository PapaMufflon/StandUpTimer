using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StandUpTimer.Web.Startup))]
namespace StandUpTimer.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
