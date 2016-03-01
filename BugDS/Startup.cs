using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BugDS.Startup))]
namespace BugDS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
