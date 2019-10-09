using System.Collections.Generic;

namespace Members.Models
{
    public class Member
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool Notify { get; set; }

        public string Note { get; set; }

        public bool Active { get; set; }

        public int ToastmastersId { get; set; }

        public string Rank { get; set; }

        public bool Deleted { get; set; }

        public string Alias { get; set; }

        public string Email2 { get; set; }

        public bool Notify2 { get; set; }

        public string Email3 { get; set; }

        public bool Notify3 { get; set; }
    }
}
