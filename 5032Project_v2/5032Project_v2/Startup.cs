using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_5032Project_v2.Startup))]
namespace _5032Project_v2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
