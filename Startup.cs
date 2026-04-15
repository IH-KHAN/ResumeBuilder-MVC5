using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ResumeBuilder_1291763.Startup))]
namespace ResumeBuilder_1291763
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
