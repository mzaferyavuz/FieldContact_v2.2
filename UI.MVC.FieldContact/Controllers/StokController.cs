using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using UI.MVC.FieldContact.Models;
using UI.MVC.FieldContact.PageModel.Management;

namespace UI.MVC.FieldContact.Controllers
{
    public class StokController : BaseController
    {
        // GET: Stok
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProducts(DataTableModel<ProductModel> model)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.Products.AsQueryable();


                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        context = context.Where(x => x.Name.Contains(model.sSearch));
                    }


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<Product> orderQuery = null;
                    if (model.iSortingCols < 1)
                    {
                        orderQuery = context.OrderBy(x => x.ID);
                    }

                    for (int i = 0; i < model.iSortingCols; i++)
                    {
                        var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                        var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                        var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                        var parametre = Expression.Parameter(typeof(Product), "x");
                        var property = typeof(Product).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<Product, object>>(memberAccess, parametre);

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
                    model.aaData = context.Select(x => new ProductModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Description = x.Description,
                        UnitsInStock = x.UnitsInStock,
                        UnitsOrder = x.Orders.Count == 0 ? 0 : ((int)x.Orders.Sum(a => a.OrderQuantity)) - ((int)x.Orders.Sum(b => b.Delivered)),
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

        public JsonResult ProductOperation(string OperationType, ProductModel Data)
        {
            var resultModel = new JsonResultModel<Product>();
            if (OperationType == "AddOrUpdate")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.Products.FirstOrDefault(x => x.ID == Data.ID) ?? new Product();
                        item.UnitsInStock = Data.UnitsInStock;

                        if (Data.ID == 0)
                        {
                            db.Context.Products.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Düzenleme Başarılı";
                    }
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "Düzenleme Başarısız. Tekrar Deneyiniz";
                }
            }



            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }
    }
}