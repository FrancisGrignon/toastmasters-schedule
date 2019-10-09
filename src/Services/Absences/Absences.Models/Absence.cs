using System;

namespace Absences.Models
{
    public class Absence : IEntity
    {
        public int Id { get; set; }

        public string Member { get; set; }

        public int MemberId { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Deleted { get; set; }
    }
}
