using System;

namespace Meetings.Models
{
    public class Meeting
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public DateTime Date { get; set; }

        public bool Active { get; set; }
    }
}
