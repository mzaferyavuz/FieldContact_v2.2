using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel
{
    public class DailyFormModel : DailyForm
    {
        public string Lati { get; set; }
        public string Longi { get; set; }
        public DateTime SelectedDate { get; set; }


        public string Name { get; set; }
        public string LastName { get; set; }
        public string Operation { get; set; }
        public string Santiye { get; set; }



        public List<CustomFramework.Job> Jobs { get; set; }
        public List<EmployeeDetail> Employees { get; set; }
        public List<CustomFramework.Car> Cars { get; set; }

        public string Brand { get; set; }
        public string CarName { get; set; }
    }
}