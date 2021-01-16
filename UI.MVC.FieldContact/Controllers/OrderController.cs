using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UI.MVC.FieldContact.Models;
using UI.MVC.FieldContact.PageModel.Orders;

namespace UI.MVC.FieldContact.Controllers
{
    public class OrderController : BaseController
    {
        // GET: Order
        public ActionResult Index()
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var pageModel = new OrderRequestModel
                    {
                        Companies=db.Context.Companies.ToList(),
                        Products=db.Context.Products.ToList()
                    };
                    return View(pageModel);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public JsonResult GetOrderRequest(DataTableModel<OrderRequestModel> model)
        {
            model.iSortingCols = 0;
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.OrderRequests.AsQueryable();


                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        context = context.Where(x => x.Company.Name.Contains(model.sSearch));
                    }


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<OrderRequest> orderQuery = null;
                    if (model.iSortingCols < 1)
                    {
                        orderQuery = context.OrderBy(x => x.ID);
                    }

                    for (int i = 0; i < model.iSortingCols; i++)
                    {
                        var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                        var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                        var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                        var parametre = Expression.Parameter(typeof(OrderRequest), "x");
                        var property = typeof(OrderRequest).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<OrderRequest, object>>(memberAccess, parametre);

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
                    model.aaData = context.Select(x => new OrderRequestModel()
                    {
                        ID = x.ID,
                        ProductName = x.Product.Name,
                        
                        CompanyName = x.Company.Name,
                        EmployeeName =x.EmployeeDetail.Name+" "+x.EmployeeDetail.LastName, /*string.Format("{0} {1}", x.EmployeeDetail.Name, x.EmployeeDetail.LastName),*/
                        Quantity = x.Quantity,
                        Operation = "<div class=\"btn-group-vertical\" style=\"position:absolute!important;\">"
                                  + "    <button class=\"btn btn-xs green dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\" aria-expanded=\"true\">"
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
                                  + "        <li>"
                                  + "            <a class=\"approveButton\" data-toggle=\"modal\" href=\"#\">"
                                  + "                <i class=\"icon-pencil\"></i> Onayla"
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

        public JsonResult OrderRequestOperation(string OperationType, OrderRequestModel Data)
        {
            var resultModel = new JsonResultModel<OrderRequest>();
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

                        var item = db.Context.OrderRequests.FirstOrDefault(x => x.ID == Data.ID) ?? new OrderRequest();
                        item.RequesterID = user.ID;
                        item.CompanyID = Data.CompanyID;
                        item.ProductID = Data.ProductID;
                        item.Quantity = Data.Quantity;

                        if (Data.ID == 0)
                        {
                            db.Context.OrderRequests.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Yeni Ürün Kaydedildi";
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

                        var item = db.Context.OrderRequests.SingleOrDefault(x => x.ID == Data.ID);
                        item.RequesterID = user.ID;
                        item.Quantity = Data.Quantity;

                        if (Data.ID == 0)
                        {
                            db.Context.OrderRequests.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Sipariş Düzenleme Başarılı";
                    }
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "İşlem Gerçekleştirilemedi";
                }
            }


            if (OperationType == "Remove")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.OrderRequests.FirstOrDefault(x => x.ID == Data.ID);
                        db.Context.OrderRequests.Remove(item);
                        db.Context.SaveChanges();
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


            if (OperationType == "Approve")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.OrderRequests.FirstOrDefault(x => x.ID == Data.ID);
                        db.Context.Orders.Add(new Order
                        {
                             CompanyID=item.CompanyID,
                              OrderQuantity=item.Quantity,
                               EmployeeID=item.RequesterID,
                                OrderDate=DateTime.Now,
                                 ProductID=item.ProductID,
                                  
                        });
                        db.Context.OrderRequests.Remove(item);
                        db.Context.SaveChanges();
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

        public JsonResult OrderApproval(OrderRequestModel Data)
        {
            var resultModel = new JsonResultModel<OrderRequest>();
            try
            {
                using (DataService db = new DataService())
                {


                    var item = db.Context.OrderRequests.FirstOrDefault(x => x.ID == Data.ID);
                    var newOrder = db.Context.Orders.Add(new Order());
                    newOrder.OrderQuantity = item.Quantity;
                    newOrder.ProductID = item.ProductID;
                    newOrder.CompanyID = item.CompanyID;
                    newOrder.EmployeeID = item.RequesterID;
                    newOrder.OrderDate = DateTime.Now;
                    newOrder.Delivered = 0;
                    newOrder.SalePrice = 0;
                    newOrder.ContQuantity = 0;
                    db.Context.OrderRequests.Remove(item);
                    db.Context.SaveChanges();
                }

                resultModel.Status = JsonResultType.Success;
                resultModel.Message = "Sipariş Talebi Onaylanarak Siparişlere Eklendi";
            }
            catch (Exception ex)
            {
                resultModel.Status = JsonResultType.Error;
                resultModel.Message = "İşlem Gerçekleştirilemedi";
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
    }
}