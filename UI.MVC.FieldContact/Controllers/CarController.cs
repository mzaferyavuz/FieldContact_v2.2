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
using UI.MVC.FieldContact.PageModel.Cars;

namespace UI.MVC.FieldContact.Controllers
{
    public class CarController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCars(DataTableModel<CarModel> model)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.Cars.AsQueryable();


                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        context = context.Where(x => x.Name.Contains(model.sSearch));
                    }


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<Car> orderQuery = null;
                    if (model.iSortingCols < 1)
                    {
                        orderQuery = context.OrderBy(x => x.ID);
                    }

                    for (int i = 0; i < model.iSortingCols; i++)
                    {
                        var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                        var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                        var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                        var parametre = Expression.Parameter(typeof(Car), "x");
                        var property = typeof(Car).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<Car, object>>(memberAccess, parametre);

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
                    model.aaData = context.Select(x => new CarModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        CarBrand = x.CarBrand,
                        HiredDate = x.HiredDate,
                        ContractDate = x.ContractDate,
                        HiredKm = x.HiredKm,
                        ContractKm = x.ContractKm,
                        ActualKm = x.ActualKm,
                        Operation = "<div class=\"btn-group-vertical\" style=\"position:absolute!important;\">"
                                  + "    <button class=\"btn btn-xs green dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\" aria-expanded=\"true\">"
                                  + "        Actions"
                                  + "        <i class=\"fa fa-angle-down\"></i>"
                                  + "    </button>"
                                  + "    <ul class=\"dropdown-menu\" role=\"menu\">"
                                  + "        <li>"
                                  + "            <a class=\"updateButton\" data-toggle=\"modal\" href=\"#responsive\">"
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


        public JsonResult CarOperation(string OperationType, CarModel Data)
        {
            var resultModel = new JsonResultModel<Car>();
            if (OperationType == "AddOrUpdate")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.Cars.FirstOrDefault(x => x.ID == Data.ID) ?? new Car();
                        item.ActualKm = Data.ActualKm;

                        if (Data.ID == 0)
                        {
                            db.Context.Cars.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Yeni Araç Kaydedildi";
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

        public ActionResult History()
        {
            return View();
        }

        public JsonResult PastCarHistory(DataTableModel<DailyFormModel> model, DateTime dateData)
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
                        Name = x.EmployeeDetail.Name + " " + x.EmployeeDetail.LastName,
                        CarName = x.Car.Name,
                        Brand = x.Car.CarBrand


                    }).ToArray();
                }
            }
            catch (Exception ex)
            {

            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CarRequest()
        {

            return View();

        }


        public JsonResult GetCarsForRequest(DataTableModel<CarRequestModel> model, DateTime dateData)
        {
            model.iSortingCols = 0;
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.Cars.AsQueryable();

                    //context = context.Where(x => x.Date == dateData.Date);  
                    //search
                    //if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    //{
                    //    context = context.Where(x => x.Name.Contains(model.sSearch));
                    //}


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<Car> orderQuery = null;
                    if (model.iSortingCols < 1)
                    {
                        orderQuery = context.OrderBy(x => x.ID);
                    }

                    for (int i = 0; i < model.iSortingCols; i++)
                    {
                        var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                        var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                        var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                        var parametre = Expression.Parameter(typeof(Car), "x");
                        var property = typeof(Car).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<Car, object>>(memberAccess, parametre);

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
                    model.aaData = context.Select(x => new CarRequestModel()
                    {
                        ID = x.ID,
                        CarName = x.Name,
                        Status = x.CarSchedules.Any(y => y.Date == dateData) ? "Rezerve" : "Boş",
                        Operation = "<a href=\"javascript:; \" class=\"btn btn-circle btn-icon-only yellow reserveCar\">"
                                  + "    <i class=\"fa fa-search\"></i>"
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

        [HttpPost]
        public JsonResult SaveCarRequest(CarRequestModel Data, DateTime dateData)
        {
            var resultModel = new JsonResultModel<CarRequest>();
            try
            {
                using (DataService db = new DataService())
                {
                    var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    var gCode = Guid.Parse(ticket.UserData);

                    var item = new CarRequest();
                    var user = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);
                    item.EmployeID = user.ID;
                    item.Date = dateData;
                    item.CarID = Data.ID;
                    item.RequestDate = DateTime.Now;

                    db.Context.CarRequests.Add(item);

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