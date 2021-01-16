using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UI.MVC.FieldContact.Models;
using UI.MVC.FieldContact.PageModel.Project;

namespace UI.MVC.FieldContact.Controllers
{
    public class ProjectController : BaseController
    {
        // GET: Project
        public ActionResult Index()
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var pageModel = new ProjectPageModel
                    {
                        Products = db.Context.Products.ToList()
                        //CompanyEmployee = db.CompEmployees.Select(x => x.Name).ToArray()
                    };
                    return View(pageModel);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public JsonResult GetAllProject(DataTableModel<ProjectPageModel> model)
        {
            using (DataService db = new DataService())
            {
                try
                {
                    var contex = db.Context.Companies.AsQueryable();


                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        contex = contex.Where(x => x.Name.Contains(model.sSearch));
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
                    model.aaData = contex.Select(x => new ProjectPageModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        ProjectStart = x.ProjectStart,
                        PlannedEndDate = x.PlannedEndDate,
                        //EndDate =x.CommissioningDate==null? (DateTime)x.CommissioningDate:DateTime.Now,
                        EndDate = x.EndDate,
                        Teslim = (((int)x.Orders.Sum(a => a.Delivered)) * 100) / ((int)x.Orders.Sum(b => b.OrderQuantity)),
                        Detaylar = "<a class=\"btn btn-circle btn-sm green viewDetails\">"
                             + "    Detaylar"
                             + "    <i class=\"fa fa - user\"></i>"
                             + "</a>"
                    }).ToArray();
                }
                catch (Exception ex)
                {

                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetFinishedProjects(DataTableModel<ProjectPageModel> model)
        {
            using (DataService db = new DataService())
            {
                try
                {
                    var contex = db.Context.Companies.AsQueryable();
                    var any = contex.Select(x => x.EndDate).ToList();
                    contex = contex.Where(x => x.EndDate != null);

                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        contex = contex.Where(x => x.Name.Contains(model.sSearch));
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
                    model.aaData = contex.Select(x => new ProjectPageModel()
                    {
                        ID=x.ID,
                        Name = x.Name,
                        ProjectStart = x.ProjectStart,
                        PlannedEndDate = x.PlannedEndDate,
                        EndDate = x.EndDate,
                        Teslim = (((int)x.Orders.Sum(a => a.Delivered)) * 100) / ((int)x.Orders.Sum(b => b.OrderQuantity)),
                        Detaylar = "<a class=\"btn btn-circle btn-sm green viewDetails\">"
                             + "    Detaylar"
                             + "    <i class=\"fa fa - user\"></i>"
                             + "</a>"
                    }).ToArray();
                }
                catch (Exception ex)
                {

                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCurrentProjects(DataTableModel<ProjectPageModel> model)
        {
            using (DataService db = new DataService())
            {
                try
                {
                    var contex = db.Context.Companies.AsQueryable();
                    var nulll = contex.Where(x => x.EndDate == null);
                    contex = contex.Where(x => x.EndDate == null);

                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        contex = contex.Where(x => x.Name.Contains(model.sSearch));
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
                    model.aaData = contex.Select(x => new ProjectPageModel()
                    {
                        ID=x.ID,
                        Name = x.Name,
                        ProjectStart = x.ProjectStart,
                        PlannedEndDate = x.PlannedEndDate,
                        EndDate = x.EndDate,
                        Teslim = (((int)x.Orders.Sum(a => a.Delivered)) * 100) / ((int)x.Orders.Sum(b => b.OrderQuantity)),
                        Detaylar = "<a class=\"btn btn-circle btn-sm green viewDetails\">"
                             + "    Detaylar"
                             + "    <i class=\"fa fa - user\"></i>"
                             + "</a>"
                    }).ToArray();
                }
                catch (Exception ex)
                {

                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetDetails(DataTableModel<ProjectOrderModel> model, int ID)
        {
            //int selectedID = Convert.ToInt32(ID);
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.Orders.AsQueryable();
                    int sayi = db.Context.Orders.Count();
                    context = context.Where(x => x.CompanyID == ID);
                    //search
                    //if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    //{
                    //    context = context.Where(x => x.Name.Contains(model.sSearch));
                    //}


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<Order> orderQuery = null;
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
                        var property = typeof(Order).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<Order, object>>(memberAccess, parametre);

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
                    model.aaData = context.Select(x => new ProjectOrderModel()
                    {
                        ID=x.ID,
                        Name = x.Product.Name,
                        ContQuantity = x.ContQuantity,
                        OrderQuantity = x.OrderQuantity,
                        Delivered = x.Delivered,
                        Description = x.Product.Description,
                        Cost = (float)x.Product.Cost,
                        ContTotalCost = (float)x.ContQuantity * (float)x.Product.Cost,
                        OrderTotalCost = (float)x.OrderQuantity * (float)x.Product.Cost,
                        SalePrice = x.SalePrice,
                        ContTotalPrice = (float)x.ContQuantity * (float)x.SalePrice,
                        OrderTotalPrice = (float)x.OrderQuantity * (float)x.SalePrice,
                        Operation = "<div class=\"btn-group-vertical\" style=\"position:absolute!important;\">"
                                  + "    <button class=\"btn btn-xs green dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\" aria-expanded=\"false\">"
                                  + "        Actions"
                                  + "        <i class=\"fa fa-angle-down\"></i>"
                                  + "    </button>"
                                  + "    <ul class=\"dropdown-menu\" role=\"menu\">"
                                  + "        <li>"
                                  + "            <a class=\"deleteButton\" href=\"javascript:; \">"
                                  + "                <i class=\"icon-trash\"></i> Sil"
                                  + "            </a>"
                                  + "        </li>"
                                  + "        <li>"
                                  + "            <a class=\"updateButton\" data-toggle=\"modal\" href=\"#responsiveEdit\">"
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

        public JsonResult ProjectOrderOperation(string OperationType, ProjectOrderModel Data)
        {
            var resultModel = new JsonResultModel<Order>();
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

                        var item = db.Context.Orders.FirstOrDefault(x => x.ID == Data.ID) ?? new Order();
                        item.CompanyID = Data.CompanyID;
                        item.ProductID = Data.ProductID;
                        item.OrderDate = DateTime.Now;
                        item.ContQuantity = Data.ContQuantity;
                        item.OrderQuantity = Data.OrderQuantity;
                        item.SalePrice = Data.SalePrice;
                        item.Delivered = Data.Delivered;
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

            if (OperationType=="Update")
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
                        item.ContQuantity = Data.ContQuantity;
                        item.OrderQuantity = Data.OrderQuantity;
                        item.SalePrice = Data.SalePrice;
                        item.Delivered = Data.Delivered;
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