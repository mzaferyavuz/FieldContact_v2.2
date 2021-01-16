using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Management
{
    public class ProductModel:Product
    {
        public int UnitsOrder { get; set; }
        public string Operation { get; set; }
    }
}