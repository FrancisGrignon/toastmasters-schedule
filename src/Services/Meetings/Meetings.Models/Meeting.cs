using System;
using System.Collections.Generic;

namespace Meetings.Models
{
    public class Meeting : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public DateTime Date { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<Attendee> Attendees { get; set; }
    }
}
