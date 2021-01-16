using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Project
{
    public class ProjectOrderModel : Order
    {
        public string Name { get; set; }
        public float OrderTotalPrice { get; set; }
        public float ContTotalPrice { get; set; }
        public float OrderTotalCost { get; set; }
        public float ContTotalCost { get; set; }
        public float Cost { get; set; }
        public string Description { get; set; }
        public string Operation { get; set; }
    }
}