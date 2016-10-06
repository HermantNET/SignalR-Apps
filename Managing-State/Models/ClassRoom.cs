using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Managing_State.Models
{
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
}