using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Job
{
    public class AvaliableEmpModel:EmployeeDetail
    {
        public int id { get; set; }
        public string text { get; set; }
    }

    public class EmpModel:EmployeeDetail
    {
        public AvaliableEmpModel[] aaData { get; set; }
    }
}