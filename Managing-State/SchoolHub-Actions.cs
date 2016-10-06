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
        public async Task CreateClassRoom(string classRoomName)
        {
            var teacher = People.Find(person => person.ConnectionId == Context.ConnectionId);
            await Groups.Add(Context.ConnectionId, classRoomName);
            // Add new room to list of active rooms, assign user as teacher, declare maximum students
            ClassRooms.Add(new ClassRoom(classRoomName, teacher, 3));
            teacher.CurrentClassRoom = classRoomName;
            // Send class information to clients
            Clients.Group(classRoomName).classRoomDetails(classRoomName, teacher.Name, "none");
        }

        public async Task JoinClassRoom(string classRoomName)
        {
            // Check if classroom exits
            if (ClassRooms.Exists(room => room.Subject == classRoomName))
            {
                await Groups.Add(Context.ConnectionId, classRoomName);
                UpdateClassStudents(classRoomName);
            }
            else
            {
                // Tell user room doesn't exist
                Clients.Caller.roomDoesntExist();
            }
        }

        public void LeaveClassRoom()
        {
            var person = People.Find(p => p.ConnectionId == Context.ConnectionId);
            var classRoom = ClassRooms.Find(room => room.Subject == person.CurrentClassRoom);

            // Check if teacher has left room
            if (classRoom.Teacher == person)
            {
                // If the classroom isn't empty, assign first student to teacher role
                if (classRoom.Students.Count != 0)
                {
                    var newTeacher = classRoom.Students.First();
                    classRoom.Teacher = newTeacher;
                    classRoom.Students.Remove(newTeacher);

                    // Update connected clients with new teacher and students property
                    UpdateClassTeacherStudents(classRoom.Subject);
                }
            }

            // Remove user from room and sets their current room to empty
            Groups.Remove(Context.ConnectionId, classRoom.Subject);
            person.CurrentClassRoom = string.Empty;
        }
    }
}