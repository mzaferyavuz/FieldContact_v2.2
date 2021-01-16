using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UI.MVC.FieldContact.Models;
using UI.MVC.FieldContact.PageModel.Job;

namespace UI.MVC.FieldContact.Controllers
{
    public class JobController : BaseController
    {
        // GET: Job
        public ActionResult Index()
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var pageModel = new JobPageModel
                    {
                        Companies = db.Context.Companies.ToList()
                    };
                return View(pageModel);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult AvaliablePersonel(DateTime date, EmpModel model)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var sche = db.Context.Schedules.Where(x => x.Date == date).AsQueryable();
                    var emp = sche.Select(x => x.EmployeeDetail.ID).ToList();
                    var avaEmp = db.Context.EmployeeDetails.Where(x=>x.RowStatusID!=3).AsQueryable();
                    
                    foreach (var item in emp)
                    {
                        avaEmp = avaEmp.Where(x => x.ID != item);
                        //var outEmp = db.Context.EmployeeDetails.Where(x => x.ID != item);
                        //avaEmp = outEmp;
                    }

                    model.aaData= avaEmp.Select(x=> new AvaliableEmpModel()
                    {
                        ID=x.ID,
                        Name=x.Name,
                        LastName=x.LastName,
                        id=x.ID,
                        text= x.Name+ " " +x.LastName
                    }).ToArray();
                    //var avaEmps = db.Context.EmployeeDetails.Except()
                    //var emp = db.Context.Schedules.Select(x=>x.EmployeeDetail).Where(y=>y.)
                }
            }
            catch (Exception ex)
            {
                
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetAllJobs (DataTableModel<JobPageModel> model)
        {
            using (DataService db = new DataService())
            {
                try
                {
                    var contex = db.Context.Jobs.AsQueryable();


                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        contex = contex.Where(x => x.JobName.Contains(model.sSearch));
                    }

                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = contex.Count();

                    contex.OrderBy(x => x.ID);

                    //paging
                    if (model.iDisplayLength > 0)
                    {
                        contex = contex.OrderBy(x => x.ID).Skip(model.iDisplayStart).Take(model.iDisplayLength);
                    }


                    //select
                    model.aaData = contex.Select(x => new JobPageModel()
                    {
                        ID = x.ID,
                        JobName = x.JobName,
                        Priority=x.Priority,
                        JobDescrition=x.JobDescrition,
                        CompanyName=x.Company.Name,
                        EstimatedWorkForce=x.EstimatedWorkForce,
                        UsedWorkForce=x.UsedWorkForce,
                        JobType=x.JobType,
                        CreatorName=x.EmployeeDetail.Name,
                        JobCreationDate = x.JobCreationDate,
                        DatetoFinish = x.DatetoFinish,
                        //EndDate =x.CommissioningDate==null? (DateTime)x.CommissioningDate:DateTime.Now,
                        JobEndDate = x.JobEndDate,
                        Detaylar = "<div class=\"btn-group-vertical\" style=\"position:absolute!important;\">"
                                  + "    <button class=\"btn btn-xs green dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\" aria-expanded=\"true\">"
                                  + "        Actions"
                                  + "        <i class=\"fa fa-angle-down\"></i>"
                                  + "    </button>"
                                  + "    <ul class=\"dropdown-menu\" role=\"menu\">"
                                  + "        <li>"
                                  + "            <a class=\"deleteButton\" href=\"#\">"
                                  + "                <i class=\"icon-trash\"></i> Sil"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"updateButton\" data-toggle=\"modal\" href=\"#responsive\">"
                                  + "                <i class=\"icon-pencil\"></i> Güncelle"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"approveButton\" data-toggle=\"modal\" href=\"#\">"
                                  + "                <i class=\"icon-pencil\"></i> Tamamlandı"
                                  + "            </a>"
                                  + "        </li>" 
                                  + "        <li>"
                                  + "            <a class=\"assignButton\" data-toggle=\"modal\" href=\"#\">"
                                  + "                <i class=\"icon-pencil\"></i> Atama Yap"
                                  + "            </a>"
                                  + "        </li>"
                                  + "    </ul>"
                                  + "</div>"
                    }).ToArray();
                }
                catch (Exception ex)
                {

                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFinishedJobs(DataTableModel<JobPageModel> model)
        {
            using (DataService db = new DataService())
            {
                try
                {
                    var contex = db.Context.Jobs.AsQueryable();
                    var any = contex.Select(x => x.JobEndDate).ToList();
                    contex = contex.Where(x => x.JobEndDate != null);

                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        contex = contex.Where(x => x.JobName.Contains(model.sSearch));
                    }

                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = contex.Count();

                    contex.OrderBy(x => x.ID);

                    //paging
                    if (model.iDisplayLength > 0)
                    {
                        contex = contex.OrderBy(x => x.ID).Skip(model.iDisplayStart).Take(model.iDisplayLength);
                    }


                    //select
                    model.aaData = contex.Select(x => new JobPageModel()
                    {
                        ID = x.ID,
                        JobName = x.JobName,
                        Priority=x.Priority,
                        JobDescrition=x.JobDescrition,
                        CompanyName=x.Company.Name,
                        EstimatedWorkForce=x.EstimatedWorkForce,
                        UsedWorkForce=x.UsedWorkForce,
                        JobType=x.JobType,
                        CreatorName=x.EmployeeDetail.Name,
                        JobCreationDate = x.JobCreationDate,
                        DatetoFinish = x.DatetoFinish,
                        JobEndDate = x.JobEndDate,
                        Detaylar = "<div class=\"btn-group-vertical\" style=\"position:absolute!important;\">"
                                  + "    <button class=\"btn btn-xs green dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\" aria-expanded=\"true\">"
                                  + "        Actions"
                                  + "        <i class=\"fa fa-angle-down\"></i>"
                                  + "    </button>"
                                  + "    <ul class=\"dropdown-menu\" role=\"menu\">"
                                  + "        <li>"
                                  + "            <a class=\"deleteButton\" href=\"#\">"
                                  + "                <i class=\"icon-trash\"></i> Sil"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"updateButton\" data-toggle=\"modal\" href=\"#responsive\">"
                                  + "                <i class=\"icon-pencil\"></i> Güncelle"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"approveButton\" data-toggle=\"modal\" href=\"#\">"
                                  + "                <i class=\"icon-pencil\"></i> Tamamlandı"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"assignButton\" data-toggle=\"modal\" href=\"#\">"
                                  + "                <i class=\"icon-pencil\"></i> Atama Yap"
                                  + "            </a>"
                                  + "        </li>"
                                  + "    </ul>"
                                  + "</div>"
                    }).ToArray();
                }
                catch (Exception ex)
                {

                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCurrentJobs(DataTableModel<JobPageModel> model)
        {
            using (DataService db = new DataService())
            {
                try
                {
                    var contex = db.Context.Jobs.AsQueryable();
                    var nulll = contex.Where(x => x.JobEndDate == null);
                    contex = contex.Where(x => x.JobEndDate == null);

                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        contex = contex.Where(x => x.JobName.Contains(model.sSearch));
                    }

                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = contex.Count();

                    contex.OrderBy(x => x.ID);

                    //paging
                    if (model.iDisplayLength > 0)
                    {
                        contex = contex.OrderBy(x => x.ID).Skip(model.iDisplayStart).Take(model.iDisplayLength);
                    }


                    //select
                    model.aaData = contex.Select(x => new JobPageModel()
                    {
                        ID = x.ID,
                        JobName = x.JobName,
                        Priority=x.Priority,
                        JobDescrition=x.JobDescrition,
                        CompanyName=x.Company.Name,
                        EstimatedWorkForce=x.EstimatedWorkForce,
                        UsedWorkForce=x.UsedWorkForce,
                        JobType=x.JobType,
                        CreatorName=x.EmployeeDetail.Name,
                        JobCreationDate=x.JobCreationDate,
                        DatetoFinish = x.DatetoFinish,
                        JobEndDate = x.JobEndDate,
                        Detaylar = "<div class=\"btn-group-vertical\" style=\"position:absolute!important;\">"
                                  + "    <button class=\"btn btn-xs green dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\" aria-expanded=\"true\">"
                                  + "        Actions"
                                  + "        <i class=\"fa fa-angle-down\"></i>"
                                  + "    </button>"
                                  + "    <ul class=\"dropdown-menu\" role=\"menu\">"
                                  + "        <li>"
                                  + "            <a class=\"deleteButton\" href=\"#\">"
                                  + "                <i class=\"icon-trash\"></i> Sil"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"updateButton\" data-toggle=\"modal\" href=\"#responsive\">"
                                  + "                <i class=\"icon-pencil\"></i> Güncelle"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"approveButton\" data-toggle=\"modal\" href=\"#\">"
                                  + "                <i class=\"icon-pencil\"></i> Tamamlandı"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"assignButton\" data-toggle=\"modal\" href=\"#\">"
                                  + "                <i class=\"icon-pencil\"></i> Atama Yap"
                                  + "            </a>"
                                  + "        </li>"
                                  + "    </ul>"
                                  + "</div>"
                    }).ToArray();
                }
                catch (Exception ex)
                {

                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JobOperation(string OperationType, JobPageModel Data)
        {
            var resultModel = new JsonResultModel<Job>();
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

                        var item = db.Context.Jobs.FirstOrDefault(x => x.ID == Data.ID) ?? new Job();
                        item.DatetoFinish = Data.DatetoFinish;
                        item.EstimatedWorkForce = Data.EstimatedWorkForce;
                        item.JobCreationDate = DateTime.Now;
                        item.JobCreatorID = user.ID;
                        item.JobDescrition = Data.JobDescrition;
                        //item.JobEndDate = Data.JobEndDate;
                        item.JobName = Data.JobName;
                        item.JobType = Data.JobType;
                        item.Priority = Data.Priority;
                        item.CompanyID = Data.CompanyID;

                        if (Data.ID == 0)
                        {
                            db.Context.Jobs.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "İş Kaydedildi";
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

                        var item = db.Context.Jobs.FirstOrDefault(x => x.ID == Data.ID) ?? new Job();
                        item.JobName = Data.JobName;
                        item.CompanyID = Data.CompanyID;
                        item.DatetoFinish = Data.DatetoFinish;
                        item.EstimatedWorkForce = Data.EstimatedWorkForce;
                        item.JobDescrition = Data.JobDescrition;
                        item.JobType = Data.JobType;
                        item.Priority = Data.Priority;
                        

                        if (Data.ID == 0)
                        {
                            db.Context.Jobs.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Güncelleme Tamamlandı";
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
                        var item = db.Jobs.FirstOrDefault(x => x.ID == Data.ID);
                        db.Jobs.Remove(item);
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

            if (OperationType == "Done")
            {
                try
                {
                    using (DataContext db = new DataContext())
                    {
                        var item = db.Jobs.FirstOrDefault(x => x.ID == Data.ID);
                        item.JobEndDate = DateTime.Now;
                        item.Priority = "Tamamlandı";
                        db.SaveChanges();
                    }
                    resultModel.Status = JsonResultType.Success;
                    resultModel.Message = "İş Tamamlandı";
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "İş Bitimi Onaylanamadı";
                }
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDetails(DataTableModel<JobScheduleModel> model, int ID)
        {
            //int selectedID = Convert.ToInt32(ID);
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.Schedules.AsQueryable();
                    int sayi = db.Context.Schedules.Count();
                    context = context.Where(x => x.JobID == ID);
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

                        var parametre = Expression.Parameter(typeof(Order), "x");
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
                    model.aaData = context.Select(x => new JobScheduleModel()
                    {
                        ID = x.ID,
                        JobName = x.Job.JobName,
                        EmployeeName = x.EmployeeDetail.Name,
                        Atender = x.EmployeeDetail1.Name,
                        Date = x.Date,
                        Operation = "<div class=\"btn-group-vertical\" style=\"position:absolute!important;\">"
                                  + "    <button class=\"btn btn-xs green dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\" aria-expanded=\"false\">"
                                  + "        Actions"
                                  + "        <i class=\"fa fa-angle-down\"></i>"
                                  + "    </button>"
                                  + "    <ul class=\"dropdown-menu\" role=\"menu\">"
                                  + "        <li>"
                                  + "            <a class=\"deleteButton2\" href=\"javascript:; \">"
                                  + "                <i class=\"icon-trash\"></i> Sil"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"updateButton2\" data-toggle=\"modal\" href=\"#responsive2\">"
                                  + "                <i class=\"icon-pencil\"></i> Güncelle"
                                  + "            </a>"
                                  + "        </li>"
                                  + "    </ul>"
                                  + "</div>"

                    }).ToArray();
                }
            }
            catch (Exception ex)
            {

            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JobScheduleOperation(string OperationType, JobScheduleModel Data)
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
                        item.Date = Data.Date;
                        item.JobID = Data.JobID;
                        item.AttenderID = user.ID;

                        if (Data.ID == 0)
                        {
                            db.Context.Schedules.Add(item);
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

                        var item = db.Context.Schedules.FirstOrDefault(x => x.ID == Data.ID) ?? new Schedule();
                        item.EmployeeID = Data.EmployeeID;
                        item.AttenderID = user.ID;
                        item.Date = Data.Date;
                        //item.JobName = Data.JobName;
                        //item.CompanyID = Data.CompanyID;
                        //item.DatetoFinish = Data.DatetoFinish;
                        //item.EstimatedWorkForce = Data.EstimatedWorkForce;
                        //item.JobDescrition = Data.JobDescrition;
                        //item.JobType = Data.JobType;
                        //item.Priority = Data.Priority;


                        if (Data.ID == 0)
                        {
                            db.Context.Schedules.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "İş Programı Değiştirildi";
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
                        var item = db.Schedules.FirstOrDefault(x => x.ID == Data.ID);
                        db.Schedules.Remove(item);
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