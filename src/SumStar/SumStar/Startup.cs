using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SumStar.Startup))]
namespace SumStar
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
