using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Schedule
{
    public class ScheduleRequestModel:CustomFramework.Schedule
    {
        public List<EmployeeDetail> Employees { get; set; }
        public List<CustomFramework.Job> Jobs { get; set; }


        public string NameSurname { get; set; }
        public string Santiye { get; set; }
        public string Operation { get; set; }
        public string Status { get; set; }
        public string Onaylayan { get; set; }
    }
}