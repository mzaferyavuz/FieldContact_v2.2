using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Cars
{
    public class CarRequestModel:CustomFramework.CarRequest
    {
        public List<CustomFramework.Car> Cars { get; set; }

        public string Status { get; set; }
        public string CarName { get; set; }
        public string Operation { get; set; }
    }
}