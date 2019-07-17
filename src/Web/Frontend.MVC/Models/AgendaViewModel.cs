using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.MVC.Models
{
    public class AgendaViewModel
    {
        public DateTime Date { get; set; }


    }

    public class Block
    {
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Member { get; set; }
        public int Duration { get; set; }

        ICollection<Labor> Labors { get; set; }
    }

    public class Labor
    {
        public string Member { get; set; }

        public int Duration { get; set; }

        public int Role { get; set; }
        
        public int Order { get; set; }
    }
}
