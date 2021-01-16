using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UI.MVC.FieldContact.Models;
using UI.MVC.FieldContact.PageModel.Schedule;

namespace UI.MVC.FieldContact.Controllers
{
    public class ScheduleController : BaseController
    {
        // GET: Schedule
        public ActionResult Index()
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var pageModel = new ScheduleModel
                    {
                        Employees = db.Context.EmployeeDetails.Where(x=>x.RowStatusID!=3).ToList(),
                        Jobs = db.Context.Jobs.ToList()
                    };
                    return View(pageModel);
                }
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        public JsonResult ScheduleTable(DataTableModel<ScheduleModel> model, DateTime dateData)
        {
            model.iSortingCols = 0;
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.Schedules.AsQueryable();

                    context = context.Where(x => x.Date == dateData.Date);
                    //search
                    //if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    //{
                    //    context = context.Where(x => x.Name.Contains(model.sSearch));
                    //}


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<Schedule> orderQuery = null;
                    if (model.iSortingCols < 1)
                    {
                        orderQuery = context.OrderBy(x => x.ID);
                    }

                    for (int i = 0; i < model.iSortingCols; i++)
                    {
                        var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                        var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                        var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                        var parametre = Expression.Parameter(typeof(Schedule), "x");
                        var property = typeof(Schedule).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<Schedule, object>>(memberAccess, parametre);

                        if (sdir == "asc")
                        {
                            orderQuery = orderQuery == null ? context.OrderBy(lambda) : orderQuery.ThenBy(lambda);
                        }
                        else
                        {
                            orderQuery = orderQuery == null ? context.OrderByDescending(lambda) : orderQuery.ThenByDescending(lambda);
                        }
                    }

                    context = orderQuery;

                    //paging
                    if (model.iDisplayLength > 0)
                    {
                        context = context.Skip(model.iDisplayStart).Take(model.iDisplayLength);
                    }


                    //sayfada gösterilecek kayıt sayısı
                    model.iTotalRecords = context.Count();

                    //select
                    model.aaData = context.Select(x => new ScheduleModel()
                    {
                        ID = x.ID,
                        NameSurname = x.EmployeeDetail.Name + " " + x.EmployeeDetail.LastName,
                        Santiye = x.Job.JobName,
                        İsiAtayan = x.EmployeeDetail1.Name,
                        Date=x.Date,
                        //AtamaTarihi = x.Date,
                        Operation = "<a data-toggle=\"modal\" href=\"#responsiveEdit \" class=\"btn btn-danger updateButton\">"
                                  + "    Düzenle "
                                  + "</a> "

                    }).ToArray();
                }
            }
            catch (Exception ex)
            {

            }


            //if (OperationType == "ListEmployees")
            //{
            //    try
            //    {
            //        using (ContactDBEntities db = new ContactDBEntities())
            //        {

            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SchedulEdit(string OperationType, ScheduleModel Data)
        {
            var resultModel = new JsonResultModel<Schedule>();
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

                        var item = db.Context.Schedules.FirstOrDefault(x => x.ID == Data.ID) ?? new Schedule();
                        item.EmployeeID = Data.EmployeeID;
                        item.JobID = Data.JobID;
                        item.Date = Data.Date;
                        item.AttenderID = user.ID;
                        //item.JobCreationDate = DateTime.Now;

                        if (Data.ID == 0)
                        {
                            db.Context.Schedules.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Yeni Firma Kaydedildi";
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

                        var item = db.Context.Schedules.SingleOrDefault(x => x.ID == Data.ID);
                        item.JobID = Data.JobID;
                        item.AttenderID = user.ID;
                        //item.JobCreationDate = DateTime.Now;

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Düzenleme Başarılı";
                    }
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "Düzenleme Bir Hatayla Karşılaştı Tekrar Deneyiniz";
                }
            }



            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ScheduleRequest()
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var pageModel = new ScheduleRequestModel
                    {
                        Employees = db.Context.EmployeeDetails.ToList(),
                        Jobs = db.Context.Jobs.ToList()
                    };
                    return View(pageModel);
                }
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        [HttpPost]
        public JsonResult SaveScheduleRequest(ScheduleRequestModel Data)
        {
            var resultModel = new JsonResultModel<Schedule>();
            try
            {
                using (DataService db = new DataService())
                {
                    var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    var gCode = Guid.Parse(ticket.UserData);

                    var item = new Schedule();
                    var user = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);
                    item.EmployeeID = user.ID;
                    item.JobID = Data.JobID;
                    item.AttenderID = user.ID;
                    //item.JobCreationDate = DateTime.Now;
                    item.Date = Data.Date;
                    
                    db.Context.Schedules.Add(item);

                    db.Context.SaveChanges();
                    resultModel.Status = JsonResultType.Success;
                    resultModel.Message = "Form Başarıyla kaydedildi. İyi çalışmalar :=) ";
                }
            }
            catch (Exception ex)
            {
                resultModel.Status = JsonResultType.Error;
                resultModel.Message = "Rapor Gönderilemedi Tekrar Deneyiniz???!!!!!";
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        
    }
}