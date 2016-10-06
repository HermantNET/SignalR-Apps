using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Managing_State
{
    public class SchoolHub : Hub
    {
        List<ClassRoom> ClassRooms { get; set; }
        List<Person> People { get; set; }
        public async Task CreateClassRoom(string classRoomName)
        {
            var teacher = People.Find(person => person.ConnectionId == Context.ConnectionId);
            await Groups.Add(Context.ConnectionId, classRoomName);
            ClassRooms.Add(new ClassRoom(classRoomName, teacher, 3));
        }
    }

    class ClassRoom
    {
        public string Subject { get; set; }
        public Person Teacher { get; set; }
        public List<Person> Students { get; set; }
        public int MaxStudents { get; set; }

        public ClassRoom(string subject, Person teacher, int max)
        {
            Subject = subject;
            Teacher = teacher;
            Students = new List<Person>();
            MaxStudents = max;
        }
    }

    class Person
    {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public string CurrentClassRoom { get; set; }

        public Person(string name, string connectionId)
        {
            Name = name;
            ConnectionId = connectionId;
            CurrentClassRoom = String.Empty;
        }
    }
}