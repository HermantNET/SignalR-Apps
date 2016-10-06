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
            People.Add(new Person("Bobby", Context.ConnectionId));
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // Remove user from the active user list
            People.RemoveAll(person => person.ConnectionId == Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}