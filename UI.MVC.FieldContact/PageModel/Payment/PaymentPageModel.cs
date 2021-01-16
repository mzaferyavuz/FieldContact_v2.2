using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Payment
{
    public class PaymentPageModel
    {
        public int PageRequesterID { get; set; }
        public List<EmployeeDetail> Employees { get; set; }
    }

    public class PaymentActivityModel:PaymentActivity
    {
        public string ActivityTypeName { get; set; }
        public decimal Tutar { get; set; }
        public string Detaylar { get; set; }
        public decimal Bakiye { get; set; }

    }

    public class PaymentActivityDetailModel:ActivityDetail
    {

    }
}