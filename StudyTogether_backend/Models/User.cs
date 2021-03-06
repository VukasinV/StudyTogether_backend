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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Profile = new HashSet<Profile>();
        }
    
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public string HashAlgorithm { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public string TokenAuth { get; set; }
        public bool TwoFaEnabled { get; set; }
        public int RoleId { get; set; }
        public Nullable<bool> Active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Profile> Profile { get; set; }
        public virtual Role Role { get; set; }
    }
}
