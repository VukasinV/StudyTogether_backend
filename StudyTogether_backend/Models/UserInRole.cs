//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudyTogether_backend.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserInRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public System.DateTime DateAssigned { get; set; }
    
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
