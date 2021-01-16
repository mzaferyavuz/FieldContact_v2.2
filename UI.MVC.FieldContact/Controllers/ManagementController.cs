using CustomFramework;
using CustomFramework.SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

using UI.MVC.FieldContact.Models;
using UI.MVC.FieldContact.PageModel.Cars;
using UI.MVC.FieldContact.PageModel.Management;

namespace UI.MVC.FieldContact.Controllers
{
    public class ManagementController : BaseController
    {

        #region ProjectManagement


        public ActionResult Projects()
        {
            return View();
        }

        public JsonResult ProjectOperation(string OperationType, ProjectModel Data)
        {
            var resultModel = new JsonResultModel<Company>();
            if (OperationType == "AddOrUpdate")
            {
                try
                {


                    using (DataService db = new DataService())
                    {
                        var item = db.Context.Companies.FirstOrDefault(x => x.ID == Data.ID) ?? new Company();
                        item.Name = Data.Name;
                        item.Address = Data.Address;
                        item.City = Data.City;
                        item.PostalCode = Data.PostalCode;
                        item.PhoneNumber = Data.PhoneNumber;
                        item.FaxNumber = Data.FaxNumber;
                        item.EmailAddress = Data.EmailAddress;
                        item.ProjectStart = Data.ProjectStart;
                        item.PlannedEndDate = Data.PlannedEndDate;
                        item.EndDate = Data.EndDate;

                        if (Data.ID == 0)
                        {
                            db.Context.Companies.Add(item);
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

            if (OperationType == "Remove")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.Companies.FirstOrDefault(x => x.ID == Data.ID);
                        db.Context.Companies.Remove(item);
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

        public JsonResult GetProjects(DataTableModel<ProjectModel> model)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.Companies.AsQueryable();


                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        context = context.Where(x => x.Name.Contains(model.sSearch));
                    }


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<Company> orderQuery = null;
                    if (model.iSortingCols < 1)
                    {
                        orderQuery = context.OrderBy(x => x.ID);
                    }

                    for (int i = 0; i < model.iSortingCols; i++)
                    {
                        var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                        var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                        var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                        var parametre = Expression.Parameter(typeof(Company), "x");
                        var property = typeof(Company).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<Company, object>>(memberAccess, parametre);

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
                    model.aaData = context.Select(x => new ProjectModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Address = x.Address,
                        City = x.City,
                        PostalCode = x.PostalCode,
                        PhoneNumber = x.PhoneNumber,
                        FaxNumber = x.FaxNumber,
                        EmailAddress = x.EmailAddress,
                        ProjectStart = x.ProjectStart.Value,
                        PlannedEndDate = x.PlannedEndDate.Value,
                        EndDate = x.EndDate,
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

        #endregion

        #region EmployeeDetailManagement


        public ActionResult Employees()
        {
            return View();
        }

        public JsonResult GetEmployees(DataTableModel<EmployeeModel> model)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.EmployeeDetails.AsQueryable();

                    context = context.Where(x => !(x.Membership_User.RowStatusID == 3));
                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        context = context.Where(x => x.Name.Contains(model.sSearch));
                    }


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<EmployeeDetail> orderQuery = null;
                    if (model.iSortingCols < 1)
                    {
                        orderQuery = context.OrderBy(x => x.ID);
                    }

                    for (int i = 0; i < model.iSortingCols; i++)
                    {
                        var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                        var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                        var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                        var parametre = Expression.Parameter(typeof(EmployeeDetail), "x");
                        var property = typeof(EmployeeDetail).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<EmployeeDetail, object>>(memberAccess, parametre);

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
                    model.aaData = context.Select(x => new EmployeeModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        LastName = x.LastName,
                        HireDate = x.HireDate,
                        BirthDate = x.BirthDate,
                        PersonelPhone = x.PersonelPhone,
                        CompPhone = x.CompPhone,
                        Salary = x.Salary,
                        Email = x.Membership_User.Email,
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

        public JsonResult EmployeeOperation(string OperationType, EmployeeModel Data)
        {
            var resultModel = new JsonResultModel<EmployeeDetail>();
            if (OperationType == "Add")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.Membership_User.FirstOrDefault(x => x.ID == Data.ID) ?? new Membership_User();
                        item.CreatedDate = DateTime.Now;
                        item.Email = Data.Email;
                        item.Username = Data.Email;
                        item.Salt = StringCipher.CreateSalt(10);
                        item.UserCode = Guid.NewGuid();
                        item.IsActive = true;
                        for (int i = 0; i < Data.Auth.Length; i++)
                        {
                            int id = Data.Auth[i];
                            var authr = db.Context.Membership_Role.FirstOrDefault(x => x.ID == id);
                            item.Membership_Role.Add(authr);
                        }

                        var itemDetail = item.EmployeeDetail = new EmployeeDetail();



                        itemDetail.BirthDate = Data.BirthDate;
                        itemDetail.CompPhone = Data.CompPhone;
                        itemDetail.HireDate = Data.HireDate;
                        itemDetail.Name = Data.Name;
                        itemDetail.LastName = Data.LastName;
                        itemDetail.PersonelPhone = Data.PersonelPhone;
                        itemDetail.Salary = Data.Salary;






                        if (Data.ID == 0)
                        {
                            db.Context.Membership_User.Add(item);
                            db.Context.EmployeeDetails.Add(itemDetail);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Yeni Eleman Kaydedildi";
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
                        var item = db.Context.Membership_User.FirstOrDefault(x => x.ID == Data.ID);
                        item.Email = Data.Email;
                        item.Username = Data.Email;
                        var itemDetail = db.Context.EmployeeDetails.FirstOrDefault(x => x.ID == item.ID);
                        itemDetail.Name = Data.Name;
                        itemDetail.LastName = Data.LastName;
                        itemDetail.PersonelPhone = Data.PersonelPhone;
                        itemDetail.Salary = Data.Salary;
                        itemDetail.HireDate = Data.HireDate;
                        itemDetail.CompPhone = Data.CompPhone;
                        itemDetail.BirthDate = Data.BirthDate;
                        if (!(Data.Auth==null))
                        {
                            for (int i = 0; i < Data.Auth.Length; i++)
                            {

                                int id = Data.Auth[i];
                                var authr = db.Context.Membership_Role.FirstOrDefault(x => x.ID == id);
                                item.Membership_Role.Add(authr);
                            }

                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Düzenleme Başarılı";

                    }
                }
                catch (Exception ex)
                {
                    resultModel.Status = JsonResultType.Error;
                    resultModel.Message = "Düzenleme İşlemi Başarısız.";
                }
            }

            if (OperationType == "Remove")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.EmployeeDetails.FirstOrDefault(x => x.ID == Data.ID);
                        db.Context.EmployeeDetails.Remove(item);
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

        #endregion



        #region ProductManagement


        public ActionResult Products()
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
                        Cost = x.Cost,
                        Description = x.Description,
                        UnitsInStock = x.UnitsInStock,
                        UnitsOrder = x.Orders.Count==0?0:((int)x.Orders.Sum(a => a.OrderQuantity)) - ((int)x.Orders.Sum(b => b.Delivered)),
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
                        item.Name = Data.Name;
                        item.Cost = Data.Cost;
                        item.Description = Data.Description;
                        item.UnitsInStock = Data.UnitsInStock;
                        item.UnitsInOrder = Data.UnitsInOrder;

                        if (Data.ID == 0)
                        {
                            db.Context.Products.Add(item);
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

            if (OperationType == "Remove")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.Products.FirstOrDefault(x => x.ID == Data.ID);
                        db.Context.Products.Remove(item);
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


        #endregion


        #region Company Employees Management


        public ActionResult CompEmployees()
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var pageModel = new CompEmployeeModel
                    {
                        Companies=db.Context.Companies.ToList()
                    };
                    return View(pageModel);
                }

            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public JsonResult GetCompEmployees(DataTableModel<CompEmployeeModel> model)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var context = db.Context.CompEmployees.AsQueryable();


                    //search
                    if (!(string.IsNullOrEmpty(model.sSearch) || string.IsNullOrWhiteSpace(model.sSearch)))
                    {
                        context = context.Where(x => x.Name.Contains(model.sSearch));
                    }


                    //toplam kayıt sayısı
                    model.iTotalDisplayRecords = context.Count();

                    //order
                    IOrderedQueryable<CompEmployee> orderQuery = null;
                    if (model.iSortingCols < 1)
                    {
                        orderQuery = context.OrderBy(x => x.ID);
                    }

                    for (int i = 0; i < model.iSortingCols; i++)
                    {
                        var scol = Request.QueryString[string.Format("iSortCol_{0}", i)];
                        var sdir = Request.QueryString[string.Format("sSortDir_{0}", i)];
                        var name = Request.QueryString[string.Format("mDataProp_{0}", scol)];

                        var parametre = Expression.Parameter(typeof(CompEmployee), "x");
                        var property = typeof(CompEmployee).GetProperty(name);
                        var memberAccess = Expression.MakeMemberAccess(parametre, property);

                        var lambda = Expression.Lambda<Func<CompEmployee, object>>(memberAccess, parametre);

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
                    model.aaData = context.Select(x => new CompEmployeeModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        LastName = x.LastName,
                        Phone = x.Phone,
                        EmailAddress = x.EmailAddress,
                        Title = x.Title,
                        EmployeeCompany = x.Company.Name,
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
                                  + "            <a class=\"updateButton\" data-toggle=\"modal\" href=\"#responsiveUpdate\">"
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

        public JsonResult CompEmployeeOperation(string OperationType, CompEmployeeModel Data)
        {
            var resultModel = new JsonResultModel<CompEmployee>();
            if (OperationType == "Add")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.CompEmployees.FirstOrDefault(x => x.ID == Data.ID) ?? new CompEmployee();
                        item.Name = Data.Name;
                        item.LastName = Data.LastName;
                        item.Phone = Data.Phone;
                        item.EmailAddress = Data.EmailAddress;
                        item.Title = Data.Title;
                        item.CompanyID = Data.CompanyID;

                        if (Data.ID == 0)
                        {
                            db.Context.CompEmployees.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Yeni Müşteri Çalışanı Kaydedildi";
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
                        var item = db.Context.CompEmployees.FirstOrDefault(x => x.ID == Data.ID) ?? new CompEmployee();
                        item.Name = Data.Name;
                        item.LastName = Data.LastName;
                        item.Phone = Data.Phone;
                        item.EmailAddress = Data.EmailAddress;
                        item.Title = Data.Title;
                        item.Company = Data.Company;

                        if (Data.ID == 0)
                        {
                            db.Context.CompEmployees.Add(item);
                        }

                        db.Context.SaveChanges();

                        resultModel.Status = JsonResultType.Success;
                        resultModel.Message = "Yeni Müşteri Çalışanı Kaydedildi";
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
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.CompEmployees.FirstOrDefault(x => x.ID == Data.ID);
                        db.Context.CompEmployees.Remove(item);
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


        #endregion



        #region Car Management
        public ActionResult Cars()
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
                                  + "            <a class=\"deleteButton\" href=\"javascript:; \">"
                                  + "                <i class=\"icon-trash\"></i> Sil"
                                  + "            </a>"
                                  + "        </li>"
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
                        item.Name = Data.Name;
                        item.CarBrand = Data.CarBrand;
                        item.HiredDate = Data.HiredDate;
                        item.ContractDate = Data.ContractDate;
                        item.HiredKm = Data.HiredKm;
                        item.ContractKm = Data.ContractKm;
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

            if (OperationType == "Remove")
            {
                try
                {
                    using (DataService db = new DataService())
                    {
                        var item = db.Context.Cars.FirstOrDefault(x => x.ID == Data.ID);
                        db.Context.Cars.Remove(item);
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
        #endregion

    }
}