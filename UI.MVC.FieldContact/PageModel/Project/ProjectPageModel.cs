using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Project
{
    public class ProjectPageModel : Company
    {
        public string Detaylar { get; set; }

        public int Teslim { get; set; }

        public List<Company> Companies { get; set; }
        public List<Product> Products { get; set; }
    }
}