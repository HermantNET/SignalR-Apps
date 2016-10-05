using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalBad
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.Caller.hello();
        }
    }
}