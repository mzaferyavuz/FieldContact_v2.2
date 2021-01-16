using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomFramework;

namespace UI.MVC.FieldContact.PageModel.Job
{
    public class JobScheduleModel : CustomFramework.Schedule
    {
        public string JobName { get; set; }
        public string EmployeeName { get; set; }
        public string Atender { get; set; }
        public string Operation { get; set; }
    }
}