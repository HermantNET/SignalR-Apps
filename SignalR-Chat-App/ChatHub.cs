using System;
using System.Web;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SignalR_Chat_App
{
    public class ChatHub : Hub
    {
        // List of currently connected users
        static List<UserConnection> userList = new List<UserConnection>();
        static JavaScriptSerializer toJson = new JavaScriptSerializer();

        public override Task OnConnected()
        {
            // Adds user to userList
            UserConnection user = new UserConnection();
            user.ConnectionString = Context.ConnectionId;
            user.Name = Context.QueryString["username"];
            user.Group = string.Empty;
            userList.Add(user);
            ConnectedUsers();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string group = userList.Find(user => user.ConnectionString == Context.ConnectionId).Group;
            // Removes user from userList on disconnect
            userList.RemoveAll(user => user.ConnectionString == Context.ConnectionId);
            ConnectedUsers();
            RoomUsers(group);
            return base.OnDisconnected(stopCalled);
        }

        public void ConnectedUsers()
        {
            // Turn users into json list. Head is 'USERS', tail is the connected users names
            string usersJson = toJson.Serialize(userList.Select(user => user.Name));
            Clients.All.connectedUsers(usersJson);
        }

        public void RoomUsers(string roomName)
        {
            var users = userList.Where(user => user.Group == roomName).Select(user => user.Name);
            // Prevents null exception in the case of a user not being in a group
            if (users.Count() != 0)
            {
                string names = toJson.Serialize(users);
                Clients.Group(roomName).inGroup(names);
            }
        }

        public async Task JoinRoom(string roomName)
        {
            var user = userList.Find(u => u.ConnectionString == Context.ConnectionId);
            if (user.Group != string.Empty)
            {
                await LeaveRoom(user.Group);
            }
                user.Group = roomName;
            
            await Groups.Add(Context.ConnectionId, roomName);
            Clients.Group(roomName).roomChanged();
        }

        public async Task LeaveRoom(string roomName)
        {
            userList.Find(user => user.ConnectionString == Context.ConnectionId).Group = String.Empty;
            await Groups.Remove(Context.ConnectionId, roomName);
            Clients.Group(roomName).roomChanged();
        }

        public Task GroupMessage(string roomName, string msg)
        {
            return Clients.Group(roomName).groupMessage("<strong>" + userList.Find(user => Context.ConnectionId == user.ConnectionString).Name + ": </strong>" + msg);
        }
    }

    class UserConnection
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string Group { get; set; }
    }
}