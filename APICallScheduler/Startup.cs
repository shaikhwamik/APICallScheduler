using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(APICallScheduler.Startup))]
namespace APICallScheduler
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
