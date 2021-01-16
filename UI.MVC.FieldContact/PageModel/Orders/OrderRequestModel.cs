using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Orders
{
    public class OrderRequestModel:CustomFramework.OrderRequest
    {
        public List<Product> Products { get; set; }
        public List<Company> Companies { get; set; }

        public string ProductName { get; set; }
        public string CompanyName { get; set; }
        public string EmployeeName { get; set; }
        public string Operation { get; set; }
    }
}