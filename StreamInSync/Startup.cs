using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(StreamInSync.Startup))]
namespace StreamInSync
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // todo: remove in prod
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true
            };
            app.MapSignalR(hubConfiguration);
        }
    }
}
