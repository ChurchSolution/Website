using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Church.Website.Startup))]
namespace Church.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
