using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyTogether_backend.Models
{
    public class ReviewDTO
    {
        public string Description { get; set; }
        public int Mark { get; set; }
        public string ReviewedUsername { get; set; }
    }
}