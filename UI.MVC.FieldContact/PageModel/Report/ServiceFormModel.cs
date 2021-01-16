using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Report
{
    public class ServiceFormModel : ServiceForm
    {
        public List<Company> Companies { get; set; }
        public List<CompEmployee> CompanyEmployees { get; set; }
        public List<CompEmpModel> ForTry { get; set; }




        public CompEmpModel[] aaData { get; set; }

        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string EmployeeName { get; set; }
        public string CoEmp1 { get; set; }
        public string CoEmp2 { get; set; }
        public string EmpSurname { get; set; }
        public string CompAddress { get; set; }
        public string CompCity { get; set; }
        public int CompPostalCode { get; set; }
        public int CompPhoneNumber { get; set; }
        public int CompFaxNumber { get; set; }
        public string CompEmail { get; set; }
        public string View { get; set; }
        public string ServisTipi { get; set; }
        public string EnddDate { get; set; }
        public string StarttDate { get; set; }
        

    }
}