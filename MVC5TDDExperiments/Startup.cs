using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC5TDDExperiments.Startup))]
namespace MVC5TDDExperiments
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
