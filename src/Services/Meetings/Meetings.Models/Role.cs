using System.Collections.Generic;

namespace Meetings.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public int Order { get; set; }

        public bool Active { get; set; }

        public ICollection<Attendee> Attendees { get; set; }
    }
}
