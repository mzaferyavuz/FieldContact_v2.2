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
    
    public partial class Job
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Job()
        {
            this.DailyForms = new HashSet<DailyForm>();
            this.Schedules = new HashSet<Schedule>();
        }
    
        public int ID { get; set; }
        public string JobName { get; set; }
        public string Priority { get; set; }
        public string JobDescrition { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public Nullable<int> EstimatedWorkForce { get; set; }
        public Nullable<int> UsedWorkForce { get; set; }
        public string JobType { get; set; }
        public Nullable<int> JobCreatorID { get; set; }
        public Nullable<System.DateTime> JobCreationDate { get; set; }
        public Nullable<System.DateTime> JobEndDate { get; set; }
        public Nullable<System.DateTime> DatetoFinish { get; set; }
    
        public virtual Company Company { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DailyForm> DailyForms { get; set; }
        public virtual EmployeeDetail EmployeeDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}