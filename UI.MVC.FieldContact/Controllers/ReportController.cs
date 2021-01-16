using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UI.MVC.FieldContact.Models;
using UI.MVC.FieldContact.PageModel;
using UI.MVC.FieldContact.PageModel.Project;
using UI.MVC.FieldContact.PageModel.Report;
// using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Newtonsoft;
using Newtonsoft.Json;

namespace UI.MVC.FieldContact.Controllers
{
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult Daily()
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var pageModel = new DailyFormModel
                    {
                        Jobs = db.Context.Jobs.ToList(),
                        Employees = db.Context.EmployeeDetails.ToList(),
                        Cars = db.Context.Cars.ToList()

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
        public JsonResult SaveDailyForm(DailyFormModel Data)
        {

            var resultModel = new JsonResultModel<DailyForm>();
            try
            {
                using (DataService db = new DataService())
                {
                    var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    var gCode = Guid.Parse(ticket.UserData);

                    var item = new DailyForm();
                    var user = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);
                    item.EmployeeID = user.ID;
                    item.JobID = Data.JobID;
                    Data.Longi = Data.Longi;
                    Data.Lati = Data.Lati;
                    item.Latitude = Data.Lati;
                    item.Latitude = Data.Latitude;
                    item.Longitude = Data.Longi;
                    item.Longitude = Data.Longitude;
                    item.Date = DateTime.Now.Date;

                    item.CarID = Data.CarID;
                    item.Description = Data.Description;
                    db.Context.DailyForms.Add(item);

                    var job = db.Context.Jobs.FirstOrDefault(x => x.ID == item.JobID);
                    if (job.UsedWorkForce==null)
                    {
                        job.UsedWorkForce = 1;
                    }
                    else
                    {
                        job.UsedWorkForce++;
                    }
                    

                    db.Context.SaveChanges();
                    resultModel.Status = JsonResultType.Success;
                    resultModel.Message = "Form Başarıyla kaydedildi. İyi çalışmalar :=) ";
                }
            }
            catch (Exception ex)
            {
                resultModel.Status = JsonResultType.Error;
                resultModel.Message = "Rapor Gönderilemedi Tekrar Deneyiniz???!!!!!" + ex.Message + ex.InnerException;
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PastDailyForms()
        {
            return View();
        }

        public JsonResult PastDailies(DataTableModel<DailyFormModel> model, DateTime dateData)
        {
            model.iSortingCols = 0;
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.DailyForms.AsQueryable();

                    context = context.Where(x => x.Date == dateData.Date);
                    //search
                    //if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    //{
                    //    context = context.Where(x => x.Name.Contains(model.sSearch));
                    //}


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<DailyForm> orderQuery = null;
                    if (model.iSortingCols < 1)
                    {
                        orderQuery = context.OrderBy(x => x.ID);
                    }

                    for (int i = 0; i < model.iSortingCols; i++)
                    {
                        var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                        var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                        var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                        var parametre = Expression.Parameter(typeof(DailyForm), "x");
                        var property = typeof(DailyForm).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<DailyForm, object>>(memberAccess, parametre);

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
                    model.aaData = context.Select(x => new DailyFormModel()
                    {
                        ID = x.ID,
                        Name = x.EmployeeDetail.Name,
                        LastName = x.EmployeeDetail.LastName,
                        Santiye = x.Job.JobName,
                        Description = x.Description,
                        Operation = "<img src='" + "https://maps.googleapis.com/maps/api/staticmap?center=" + x.Latitude + "," + x.Longitude + "&zoom=14&size=400x300&key=AIzaSyA9Da5NbXmt8mkYUrRecxGrvynNyFez4Ao" + "'>"


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

        public JsonResult ShowOnMap(string OperationType, DailyFormModel Data)
        {
            var resultModel = new JsonResultModel<DailyForm>();
            if (OperationType == "Show")
            {
                try
                {
                    using (DataService db = new DataService())
                    {


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



            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Service()
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var pageModel = new ServiceFormModel
                    {

                        Companies = db.Context.Companies.ToList(),
                        CompanyEmployees = db.Context.CompEmployees.ToList()

                        //CompanyName = "Şirket Adı",
                        //EmployeeName = "Çalışan Adı",
                        //CompAddress = "Adres",
                        //CompCity = "Şehir",
                        //CompPostalCode = 1,
                        //CompFaxNumber = 1,
                        //CompPhoneNumber = 1,
                        //CompEmail = "company@info.com",
                        //AdamGun = 2,
                        //Description = "Servis Açıklaması",
                        //StartDate = DateTime.Now,
                        //EndDate = DateTime.Now,
                        //ServisTipi = Models.ServiceType.Arıza
                    };
                    return View(pageModel);
                }
            }

            catch (Exception ex)
            {
                return null;

            }
        }

        public ActionResult History()
        {
            return View();
        }

        public JsonResult GetForms(DataTableModel<ServiceFormModel> model)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    //var context = from x in db.ServiceForms
                    //              join y in db.Employees
                    //              on x.EmployeeID equals y.ID
                    //              join z in db.Companies
                    //              on x.CompanyID equals z.ID
                    //              join w in db.ServiceTypes
                    //              on x.ServiceTypeID equals w.ID
                    //              //group x by x.ID into pg
                    //              select new
                    //              {
                    //                  x.ID,
                    //                  CompanyName=z.Name,
                    //                  EmployeeName=y.Name,
                    //                  CompAddress=z.Address,
                    //                  CompCity=z.City,
                    //                  CompPostalCode=z.PostalCode,
                    //                  CompPhoneNumber=z.PhoneNumber,
                    //                  CompFaxNumber=z.FaxNumber,
                    //                  CompEmail=z.EmailAddress,
                    //                  Description=x.ServiceDetails,
                    //                  ServisTipi=w.Name,
                    //                  AdamGun=x.Adam_Gun,
                    //                  StartDate=x.StartDate,
                    //                  EndDate=x.EndDate

                    //              };


                    //var contex = db.Database.SqlQuery (typeof(ServiceFormModel),"Select * From ServiceForms AS SF Inner Join Employees AS E ON SF.EmployeeID=E.ID " +
                    //    "INNER Join Companies AS C ON SF.CompanyID=C.ID " +
                    //    "INNER JOIN ServiceTypes AS ST ON SF.ServiceTypeID=ST.ID");


                    //var context2 = from x in db.ServiceForms
                    //               join y in context
                    //               on x.ID equals y.ID
                    //               select new
                    //               {
                    //                   StartDate = x.Date,
                    //               };


                    var contex = db.Context.ServiceForms.AsQueryable();


                    //Search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        contex = contex.Where(x => x.Company.Name.Contains(model.sSearch) || x.StartDate.ToString().Substring(0, 11).Contains(model.sSearch) || x.EndDate.ToString().Substring(0, 11).Contains(model.sSearch));
                    }


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = contex.Count();


                    contex.OrderBy(x => x.ID);
                    //Order
                    //#region Order
                    //IOrderedQueryable<ServiceForm> orderQuery = null;

                    //if (model.iSortingCols < 1)
                    //{
                    //    orderQuery = contex.OrderBy(x => x.ID);
                    //}
                    //for (int i = 0; i < model.iSortingCols; i++)
                    //{
                    //    var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                    //    var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                    //    var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                    //    var parametre = Expression.Parameter(typeof(ServiceForm), "x");
                    //    var property = typeof(ServiceForm).GetProperty(name);
                    //    var memberAccess = Expression.MakeMemberAccess(parametre, property);

                    //    var lambda = Expression.Lambda<Func<ServiceForm, object>>(memberAccess, parametre);

                    //    if (sdir == "asc")
                    //    {
                    //        //ASC
                    //        orderQuery = orderQuery == null ? contex.OrderBy(lambda) : orderQuery.ThenBy(lambda);
                    //    }
                    //    else
                    //    {
                    //        orderQuery = orderQuery == null ? contex.OrderByDescending(lambda) : orderQuery.ThenByDescending(lambda);
                    //    }
                    //}

                    //contex = orderQuery;
                    //#endregion

                    //Paging
                    if (model.iDisplayLength > 0)
                    {
                        contex = contex.OrderBy(x => x.ID).Skip(model.iDisplayStart).Take(model.iDisplayLength);
                    }

                    //sayfada gösterilecek kayıt sayısı
                    //model.iTotalDisplayRecords = contex.Count();

                    //select
                    model.aaData = contex.Select(x => new ServiceFormModel()
                    {
                        ID=x.ID,
                        CompanyName = x.Company.Name,
                        EmployeeName = x.EmployeeDetail.Name + " " + x.EmployeeDetail.LastName,
                        StarttDate = x.StartDate.ToString().Substring(0, 11),
                        EnddDate = x.EndDate.ToString().Substring(0, 11),
                        ServisTipi = x.SerialNo,
                        View = "<a href=\"javascript:; \" class=\"btn btn-circle btn-icon-only yellow viewDetails\">"
                             + "    <i class=\"fa fa-search\"></i>"
                             + "</a> "



                    }).ToArray();



                }
            }
            catch (Exception ex)
            {

            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormDetail(int id)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var form = db.Context.ServiceForms.FirstOrDefault(x => x.ID == id);
                    var model = new ServiceFormModel
                    {
                        Company = form.Company,
                        //ServisTipi = form.ServiceType.Name,
                        StartDate = form.StartDate,
                        EndDate = form.EndDate,
                        //Employee = form.Employee,
                        ServiceDetails = form.ServiceDetails
                        //Adam_Gun = form.Adam_Gun

                    };
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult GetServiceForm (ServiceFormModel model, int ID)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var form = db.Context.ServiceForms.SingleOrDefault(x=>x.ID==ID);
                    var emp = db.Context.EmployeeDetails.SingleOrDefault(x => x.ID == form.EmployeeID);
                    var coEmp1 = db.Context.CompEmployees.SingleOrDefault(x => x.ID == form.CompEmp1ID);
                    var coEmp2 = db.Context.CompEmployees.SingleOrDefault(x => x.ID == form.CompEmp2ID);
                    model.ID = form.ID;
                    model.IsArıza = form.IsArıza;
                    model.IsBakım = form.IsBakım;
                    model.IsDelivering = form.IsDelivering;
                    model.IsDevreyeAlma = form.IsDevreyeAlma;
                    model.IsEducate = form.IsEducate;
                    model.IsFirsManint = form.IsFirsManint;
                    model.IsFree = form.IsFree;
                    model.IsMeeting = form.IsMeeting;
                    model.IsMontage = form.IsMontage;
                    model.IsSecondMaint = form.IsSecondMaint;
                    model.IsSupervice = form.IsSupervice;
                    model.IsThirdMaint = form.IsThirdMaint;
                    model.IsWarrantied = form.IsWarrantied;
                    model.HVAC = form.HVAC;
                    model.JobDescription = form.JobDescription;
                    model.SerialNo = form.SerialNo;
                    model.ServiceDetails = form.ServiceDetails;
                    model.EndDate = form.EndDate;
                    model.EmployeeName = emp.Name + " " + emp.LastName;
                    model.CoEmp1 = coEmp1.Name;
                    model.CoEmp2 = coEmp2.Name;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CompanySelection(ServiceFormModel model, int ID)
        {

            try
            {
                using (DataService db = new DataService())
                {
                    model.Address = db.Context.Companies.SingleOrDefault(x => x.ID == ID).Address;
                    var oper = db.Context.CompEmployees.AsQueryable();
                    oper= oper.Where(x => x.CompanyID == ID);
                    //CompEmpModel[] coEmp = new CompEmpModel() {
                    //    ID=oper.Select(x.ID),
                    //};
                    model.aaData = oper.Select(x => new CompEmpModel()
                    {
                         ID=x.ID,
                         Name=x.Name,
                         LastName=x.LastName,
                         id=x.ID,
                         text=x.Name+" "+x.LastName
                    }).ToArray();
                    

                   
                    //for (int i = 0; i <oper.Length; i++)
                    //{
                    //    model.CoEmpId[i] = oper[i].ID;
                    //    model.ComEmp[i] = oper[i].Name + " " + oper[i].LastName;
                    //}
                    //model.CompE = coEmp;
                    //for (int i = 0; i < coEmp.Length; i++)
                    //{
                    //    model.CoEmpId.Add(coEmp[i].ID);
                    //    model.ComEmp.Add(coEmp[i].Name + " " + coEmp[i].LastName);
                    //}
                    //foreach (var item in coEmp)
                    //{
                    //    model.CoEmpId.Add(item.ID);
                    //    model.ComEmp.Add(item.Name + " " + item.LastName);
                    //}   
                }
            }
            catch (Exception ex)
            {

            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CompEmpSelection(string empInfo, int ID)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var coemp = db.Context.CompEmployees.FirstOrDefault(x => x.ID == ID);
                    string mail = coemp.EmailAddress;
                    string phone = coemp.Phone;

                    empInfo = string.Format(" {0} /  {1}", phone, mail);

                }
                return Json(empInfo,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        [HttpPost]
        public JsonResult SaveServiceForm(ServiceFormModel Data)
        {
            var resultModel = new JsonResultModel<ServiceFormModel>();
            var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var gCode = Guid.Parse(ticket.UserData);
            try
            {
                using (DataService db = new DataService())
                {
                    var serviceForm = new ServiceForm();
                    var user = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);
                    serviceForm.CompanyID = Data.CompanyID;
                    serviceForm.CompEmp1ID = Data.CompEmp1ID;
                    serviceForm.CompEmp2ID = Data.CompEmp2ID;
                    serviceForm.EmployeeID = user.ID;

                    serviceForm.IsArıza = Data.IsArıza;
                    serviceForm.IsBakım = Data.IsBakım;
                    serviceForm.IsDelivering = Data.IsDelivering;
                    serviceForm.IsDevreyeAlma = Data.IsDevreyeAlma;
                    serviceForm.IsEducate = Data.IsEducate;
                    serviceForm.IsFirsManint = Data.IsFirsManint;
                    serviceForm.IsFree = Data.IsFree;
                    serviceForm.IsMeeting = Data.IsMontage;
                    serviceForm.IsSecondMaint = Data.IsSecondMaint;
                    serviceForm.IsSupervice = Data.IsSupervice;
                    serviceForm.IsThirdMaint = Data.IsThirdMaint;
                    serviceForm.IsWarrantied = Data.IsWarrantied;
                    serviceForm.IsMontage = Data.IsMontage;
                    serviceForm.HVAC = Data.HVAC;

                    serviceForm.JobDescription = Data.JobDescription;
                    //serviceForm.SerialNo = Data.SerialNo;
                    serviceForm.ServiceDetails = Data.ServiceDetails;
                    serviceForm.StartDate = Data.StartDate;
                    serviceForm.EndDate = Data.EndDate;
                    //var compAdress = db.Context.Companies.FirstOrDefault(y => y.ID == Data.CompanyID).Address;

                    //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                    //Microsoft.Office.Interop.Excel.Workbook sheet = excel.Workbooks.Open("/Content/ServiceFormTemplate.xlsx");
                    //Microsoft.Office.Interop.Excel.Worksheet z = excel.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;


                    //Excel.Range userRange = z.UsedRange;

                    //userRange.Replace("", compAdress);
                    //userRange.Replace("", "");
                    //sheet.SaveAs("YeniForm.pdf");
                    //return sheet;
                    //System.IO.MemoryStream memory = new System.IO.MemoryStream();
                    //byte[] bytes= System.Text.Encoding.UTF8.GetBytes(sheet);
                    //RedirectToAction("ReturnPdf", ID)
                    db.Context.ServiceForms.Add(serviceForm);
                    db.Context.SaveChanges();

                    resultModel.Status = JsonResultType.Success;
                    resultModel.Message = "Yeni Sipariş Kaydedildi";
                }


            }
            catch (Exception)
            {

            }
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        //public FileStreamResult( int ID)
        //{
        //    try
        //    {
        //        using (DataService db = new DataService())
        //        {
        //            var form = db.Context.ServiceForms.FirstOrDefault(x => x.ID == ID);
        //            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        //            Microsoft.Office.Interop.Excel.Workbook sheet = excel.Workbooks.Open("/Content/ServiceFormTemplate.xlsx");
        //            Microsoft.Office.Interop.Excel.Worksheet z = excel.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;


        //            Excel.Range userRange = z.UsedRange;

        //            userRange.Replace("", form.Company.Address);
        //            userRange.Replace("", "");
        //            sheet.SaveAs("YeniForm.pdf");
        //            sheet.Close();
        //            System.IO.MemoryStream memory = new System.IO.MemoryStream();
        //            System.
        //            return new FileStreamResult()
        //        }
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }
        //}

    }
}