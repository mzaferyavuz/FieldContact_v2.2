using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace UI.MVC.FieldContact.PageModel.Management
{
    public class CompEmployeeModel:CompEmployee
    {
        public string Operation { get; set; }
        public string EmployeeCompany { get; set; }

        public List<CompEmployee> CompanyEmployees { get; set; }
        public List<Company> Companies { get; set; }
    }
}