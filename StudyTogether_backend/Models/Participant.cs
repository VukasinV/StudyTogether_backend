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
    
    public partial class Participant
    {
        public int ProfileId { get; set; }
        public int MeetingId { get; set; }
        public bool Canceled { get; set; }
    
        public virtual Meeting Meeting { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
