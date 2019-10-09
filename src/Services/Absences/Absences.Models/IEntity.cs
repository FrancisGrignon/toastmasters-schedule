using System;

namespace Absences.Models
{
    public interface IEntity
    {
        int Id { get; set; }

        bool Deleted { get; set; }

        DateTime CreatedAt { get; set; }

        DateTime UpdatedAt { get; set; }
    }
}
