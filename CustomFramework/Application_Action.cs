//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CustomFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class Application_Action
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Application_Action()
        {
            this.Membership_Role = new HashSet<Membership_Role>();
            this.Membership_User = new HashSet<Membership_User>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> ControllerID { get; set; }
    
        public virtual Application_Controller Application_Controller { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Membership_Role> Membership_Role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Membership_User> Membership_User { get; set; }
    }
}
