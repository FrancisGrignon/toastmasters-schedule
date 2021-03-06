﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meetings.API.ViewModels
{
    public class RoleRequestViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(2048)]
        public string Note { get; set; }

        public int Order { get; set; }
    }
}
