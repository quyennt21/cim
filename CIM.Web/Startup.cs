using Microsoft.Owin;
using Owin;

[assembly: OwinStartup("ProductionConfiguration", typeof(CIM.Web.Startup))]

namespace CIM.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAutofac(app);
            ConfigureAuth(app);
        }
    }
}