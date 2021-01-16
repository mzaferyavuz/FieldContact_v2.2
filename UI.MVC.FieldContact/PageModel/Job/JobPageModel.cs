using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Job
{
    public class JobPageModel:CustomFramework.Job
    {
        public string Detaylar { get; set; }

        public string CreatorName { get; set; }
        public string CompanyName { get; set; }
        public List<Company> Companies { get; set; }
    }
}