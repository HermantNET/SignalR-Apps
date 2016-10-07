using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Managing_State.Models;

namespace Managing_State
{
    public partial class SchoolHub : Hub
    {
        static List<ClassRoom> ClassRooms = new List<ClassRoom>();
        static List<Person> People = new List<Person>();

        public override Task OnConnected()
        {
            // Add the newly connected user to the active user list
            // TODO implement custom user name
            People.Add(new Person(Context.QueryString["name"], Context.ConnectionId));
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // Remove user from the active user list
            LeaveCurrentRoom();
            People.Remove(CurrentPerson());
            return base.OnDisconnected(stopCalled);
        }
    }
}