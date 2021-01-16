using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UI.MVC.FieldContact.Models;
using UI.MVC.FieldContact.PageModel.Payment;

namespace UI.MVC.FieldContact.Controllers
{
    public class PaymentController : BaseController
    {
        // GET: Payment
        public ActionResult Index()
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    var gCode = Guid.Parse(ticket.UserData);
                    var user = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);
                    var role = user.Membership_Role.Count;
                    int approve;
                    if (db.Context.Membership_User.Any(x => x.UserCode == gCode && (x.Membership_Role.Any(y => y.Application_Action.Any(z => z.Name == "IndexDetail" && z.Application_Controller.Name == "Payment")) || x.Application_Action.Any(z => z.Name == "IndexDetail" && z.Application_Controller.Name == "Payment"))))
                    {
                        approve = 1;
                    }
                    else
                    {
                        approve = 0;
                    }
                    var pageModel = new PaymentPageModel
                    {
                        PageRequesterID = approve,
                         Employees = db.Context.EmployeeDetails.Where(x=>x.RowStatusID!=3).ToList()
                    };
                    return View(pageModel);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult GetPaymentActivities(DataTableModel<PaymentActivityModel> model, int ID)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    if (ID==0)
                    {
                        var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                        var ticket = FormsAuthentication.Decrypt(cookie.Value);
                        var gCode = Guid.Parse(ticket.UserData);
                        var user = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);
                        ID = user.ID;
                    }
                    var contex = db.Context.PaymentActivities.Where(x=>x.ActivityOwnerID==ID).AsQueryable();

                    //search
                    //if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    //{
                    //    contex = contex.Where(x => x.Name.Contains(model.sSearch));
                    //}

                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = contex.Count();

                    contex.OrderBy(x => x.ID);

                    //paging
                    if (model.iDisplayLength > 0)
                    {
                        contex = contex.OrderBy(x => x.ID).Skip(model.iDisplayStart).Take(model.iDisplayLength);
                    }

                    //select
                    model.aaData = contex.Select(x => new PaymentActivityModel()
                    {
                        ID = x.ID,
                        Date = x.Date,
                        ActivityTypeName = x.ActivityType.Name,
                        Tutar = x.ActivityDetails.Count==0?0:(((decimal)x.ActivityDetails.Sum(a => a.Amount))),
                        Detaylar = "<a class=\"btn btn-circle btn-sm green viewDetails\">"
                             + "    Detaylar"
                             + "    <i class=\"fa fa - user\"></i>"
                             + "</a>"
                    }).ToArray();
                    model.dtBakiye = db.Context.ActivityDetails.Where(c => c.PaymentActivity.ActiviyTypeID == 2 && c.PaymentActivity.ActivityOwnerID == ID)
                        .Sum(d => d.Amount) - db.Context.ActivityDetails.Where(a => a.PaymentActivity.ActiviyTypeID == 1 && a.PaymentActivity.ActivityOwnerID == ID)
                        .Sum(b => b.Amount);
                }
            }
            catch (Exception ex)
            {
                
            }
            return Json(model,JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivityOperation(string OperationType, PaymentActivityModel Data)
        {
            var resultModel = new JsonResultModel<PaymentActivity>();
            if (OperationType == "Add")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                        var ticket = FormsAuthentication.Decrypt(cookie.Value);
                        var gCode = Guid.Parse(ticket.UserData);
                        var user = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);

                        var item = db.Context.PaymentActivities.FirstOrDefault(x => x.ID == Data.ID) ?? new PaymentActivity();
                        item.ActiviyTypeID = Data.ActiviyTypeID;
                        item.Date = DateTime.Now;
                        item.ActivityOwnerID =Data.ActivityOwnerID==0?user.ID: Data.ActivityOwnerID;

                        if (Data.ID == 0)
                        {
                            db.Context.PaymentActivities.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Yeni Sipariş Kaydedildi";
                    }
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "Kayıt İşlemi Gerçekleştirilemedi";
                }
            }

            if (OperationType == "Update")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                        var ticket = FormsAuthentication.Decrypt(cookie.Value);
                        var gCode = Guid.Parse(ticket.UserData);
                        var user = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);

                        var item = db.Context.Orders.FirstOrDefault(x => x.ID == Data.ID) ?? new Order();
                        item.OrderDate = DateTime.Now;
                        item.EmployeeID = user.ID;

                        if (Data.ID == 0)
                        {
                            db.Context.Orders.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Yeni Sipariş Kaydedildi";
                    }
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "Kayıt İşlemi Gerçekleştirilemedi";
                }
            }

            if (OperationType == "Remove")
            {
                try
                {
                    using (DataContext db = new DataContext())
                    {
                        var item = db.Orders.FirstOrDefault(x => x.ID == Data.ID);
                        db.Orders.Remove(item);
                        db.SaveChanges();
                    }

                    resultModel.Status = JsonResultType.Success;
                    resultModel.Message = "Silme İşlemi Başarılı";
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "Silme İşlemi Gerçekleştirilemedi";
                }
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivityDetails(DataTableModel<PaymentActivityDetailModel> model, int ID)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    
                    var contex = db.Context.ActivityDetails.Where(x => x.PaymentActivityID == ID).AsQueryable();

                    //search
                    //if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    //{
                    //    contex = contex.Where(x => x.Name.Contains(model.sSearch));
                    //}

                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = contex.Count();

                    contex.OrderBy(x => x.ID);

                    //paging
                    if (model.iDisplayLength > 0)
                    {
                        contex = contex.OrderBy(x => x.ID).Skip(model.iDisplayStart).Take(model.iDisplayLength);
                    }

                    //select
                    model.aaData = contex.Select(x => new PaymentActivityDetailModel()
                    {
                        ID = x.ID,
                        PaymentDate = x.PaymentDate,
                        Amount = x.Amount,
                        Description = x.Description
                    }).ToArray();
                }
            }
            catch (Exception ex)
            {

            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PaymentActivityOperation(string OperationType, PaymentActivityDetailModel Data)
        {
            var resultModel = new JsonResultModel<ActivityDetail>();
            if (OperationType == "Add")
            {
                try
                {
                    using (DataService db = new DataService())
                    {

                        var item = db.Context.ActivityDetails.FirstOrDefault(x => x.ID == Data.ID) ?? new ActivityDetail();
                        item.PaymentActivityID = Data.PaymentActivityID;
                        item.Description = Data.Description;
                        item.PaymentDate = Data.PaymentDate;
                        item.Amount = Data.Amount;

                        if (Data.ID == 0)
                        {
                            db.Context.ActivityDetails.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Kaydedildi";
                    }
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "Kayıt İşlemi Gerçekleştirilemedi";
                }
            }

            if (OperationType == "Update")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                        var ticket = FormsAuthentication.Decrypt(cookie.Value);
                        var gCode = Guid.Parse(ticket.UserData);
                        var user = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);

                        var item = db.Context.Orders.FirstOrDefault(x => x.ID == Data.ID) ?? new Order();
                        item.OrderDate = DateTime.Now;
                        item.EmployeeID = user.ID;

                        if (Data.ID == 0)
                        {
                            db.Context.Orders.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Yeni Sipariş Kaydedildi";
                    }
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "Kayıt İşlemi Gerçekleştirilemedi";
                }
            }

            if (OperationType == "Remove")
            {
                try
                {
                    using (DataContext db = new DataContext())
                    {
                        var item = db.Orders.FirstOrDefault(x => x.ID == Data.ID);
                        db.Orders.Remove(item);
                        db.SaveChanges();
                    }

                    resultModel.Status = JsonResultType.Success;
                    resultModel.Message = "Silme İşlemi Başarılı";
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "Silme İşlemi Gerçekleştirilemedi";
                }
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

       
    }
}