using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LibraryMongo.Startup))]
namespace LibraryMongo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
