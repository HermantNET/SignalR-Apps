using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Managing_State.Models
{
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