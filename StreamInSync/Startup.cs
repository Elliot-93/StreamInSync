using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(StreamInSync.Startup))]
namespace StreamInSync
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
