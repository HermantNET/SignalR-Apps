using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalR_Chat_App.Startup))]

namespace SignalR_Chat_App
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
