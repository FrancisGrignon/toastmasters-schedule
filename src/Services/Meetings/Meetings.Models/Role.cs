using System;
using System.Collections.Generic;

namespace Meetings.Models
{
    public class Role : IEntity
    {
        public static readonly int Improviser = 7;
        public static readonly int Member = 14;
        public static readonly int Visitor = 15;

        public int Id { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public int Order { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<Attendee> Attendees { get; set; }
    }
}
