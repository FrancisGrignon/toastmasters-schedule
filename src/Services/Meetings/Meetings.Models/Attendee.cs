using System;

namespace Meetings.Models
{
    public class Attendee : IEntity
    {
        public int Id { get; set; }

        public int MeetingId { get; set; }

        public Meeting Meeting { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }

        public int? MemberId { get; set; }

        public string Member { get; set; }

        public int Order { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
