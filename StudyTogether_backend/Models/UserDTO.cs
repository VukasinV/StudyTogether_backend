using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyTogether_backend.Models
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
    }
}