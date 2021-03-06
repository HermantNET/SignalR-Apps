﻿using System;
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
        private Person CurrentPerson()
        {
            return People.Find(p => p.ConnectionId == Context.ConnectionId);
        }

        private void LeaveCurrentRoom()
        {
            var person = CurrentPerson();
            if (person != null)
            {
                if (person.CurrentClassRoom != string.Empty)
                {
                    LeaveClassRoom();
                }
            }
        }

        private void AddToStudents(ClassRoom classroom)
        {
            classroom.Students.Add(CurrentPerson());
        }

        private void RemoveFromStudents(ClassRoom classroom)
        {
            classroom.Students.Remove(CurrentPerson());
        }

        public async Task CreateClassRoom(string classRoomName, string maxClients)
        {
            LeaveCurrentRoom();
            var teacher = CurrentPerson();
            await Groups.Add(Context.ConnectionId, classRoomName);
            int maxStudents = 5;

            // Check if maxClients is an Integer and is within an acceptable range, else set to 5
            int.TryParse(maxClients, out maxStudents);
            if (maxStudents == 0)
            {
                maxStudents = 5;
            }
            else if (maxStudents > 30)
            {
                maxStudents = 30;
            }

            // Add new room to list of active rooms, assign user as teacher, declare maximum students
            ClassRooms.Add(new ClassRoom(classRoomName, teacher, maxStudents));
            teacher.CurrentClassRoom = classRoomName;
            // Send class information to clients
            UpdateCallerAll(classRoomName);
        }

        public async Task JoinClassRoom(string classRoomName)
        {
            var classroom = ClassRooms.Find(room => room.Subject == classRoomName);
            // Check if classroom exits
            if (classroom != null)
            {
                if (classroom.MaxStudents > classroom.Students.Count)
                {
                    LeaveCurrentRoom();

                    await Groups.Add(Context.ConnectionId, classRoomName);
                    // Update Persons current classroom property
                    CurrentPerson().CurrentClassRoom = classRoomName;
                    AddToStudents(classroom);
                    UpdateCallerAll(classRoomName);
                    UpdateClassStudents(classRoomName);
                }
                else
                {
                    Clients.Caller.roomIsFull();
                }
            }
            else
            {
                // Tell user room doesn't exist
                Clients.Caller.roomDoesntExist();
            }
        }

        public void LeaveClassRoom()
        {
            var person = CurrentPerson();
            var classRoom = ClassRooms.Find(room => room.Subject == person.CurrentClassRoom);

            if (classRoom != null)
            {
                // Check if teacher has left room
                if (classRoom.Teacher == person)
                {
                    // If the classroom isn't empty, assign first student to teacher role
                    if (classRoom.Students.Count > 0)
                    {
                        var newTeacher = classRoom.Students.First();
                        classRoom.Teacher = newTeacher;
                        classRoom.Students.Remove(newTeacher);
                        Groups.Remove(Context.ConnectionId, classRoom.Subject);
                        // Update connected clients with new teacher and students property
                        UpdateClassTeacherStudents(classRoom.Subject);
                    }
                    else
                    {
                        Groups.Remove(Context.ConnectionId, classRoom.Subject);
                        ClassRooms.Remove(classRoom);
                    }
                }
                else
                {
                    Groups.Remove(Context.ConnectionId, classRoom.Subject);
                    RemoveFromStudents(classRoom);
                    UpdateClassStudents(classRoom.Subject);
                }

                person.CurrentClassRoom = string.Empty;
                Clients.Caller.leftRoom();
            }
        }
    }
}