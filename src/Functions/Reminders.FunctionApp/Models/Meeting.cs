using System;
using System.Collections.Generic;

namespace Reminders.FunctionApp.Models
{
    public class Meeting
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public DateTime Date { get; set; }

        public ICollection<Attendee> Attendees { get; set; }

        public bool Cancelled { get; set; }
    }
}
