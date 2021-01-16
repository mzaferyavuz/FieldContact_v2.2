using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomFramework;

namespace UI.MVC.FieldContact.PageModel.Management
{
    public class EmployeeModel:EmployeeDetail
    {
        public string BirthDat { get; set; }
        public string HireDat { get; set; }
        public string Email { get; set; }
        public string Operation { get; set; }
        public int[] Auth { get; set; }
    }
}