using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyTogether_backend.Models
{
    public class MeetingDTO
    {
        public string Location { get; set; }
        public DateTime StartsAt { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string CreatedBy { get; set; }
        public string[] Participants { get; set; }
    }
}