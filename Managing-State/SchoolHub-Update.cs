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
        static private string StudentsNotZero(List<Person> studentsList)
        {
            string result;

            // Determine whether classroom is empty or not
            if (studentsList.Count != 0)
            {
                result = string.Join(", ", studentsList.Select(student => student.Name));
            }
            else
            {
                result = "none";
            }

            return result;
        }

        private string StudentsCount(ClassRoom classroom)
        {
            return "Students: " + classroom.Students.Count + "/" + classroom.MaxStudents + ": ";
        }

        private void UpdateClassTeacherStudents(string classRoomName)
        {
            var classroom = ClassRooms.Find(room => room.Subject == classRoomName);
            string students = StudentsNotZero(classroom.Students);
            Clients.OthersInGroup(classRoomName).classRoomDetails(StudentsCount(classroom) + students, "Teacher: " + classroom.Teacher.Name);
        }

        private void UpdateClassStudents(string classRoomName)
        {
            var classroom = ClassRooms.Find(room => room.Subject == classRoomName);
            string students = StudentsNotZero(classroom.Students);
            Clients.OthersInGroup(classRoomName).classRoomDetails(StudentsCount(classroom) + students);
        }

        private void UpdateCallerAll(string classRoomName)
        {
            var classroom = ClassRooms.Find(room => room.Subject == classRoomName);
            string students = StudentsNotZero(classroom.Students);
            Clients.Caller.classRoomDetails(StudentsCount(classroom) + students, "Teacher: " + classroom.Teacher.Name, "Subject: " + classroom.Subject);
        }
    }
}