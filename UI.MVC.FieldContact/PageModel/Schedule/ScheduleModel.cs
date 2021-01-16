using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Schedule
{
    public class ScheduleModel :CustomFramework.Schedule
    {
        public List<EmployeeDetail> Employees { get; set; }
        public List<CustomFramework.Job> Jobs { get; set; }

        public string NameSurname { get; set; }
        public string Santiye { get; set; }
        public string İsiAtayan { get; set; }
        public DateTime AtamaTarihi { get; set; }
        public string Operation { get; set; }
    }
}