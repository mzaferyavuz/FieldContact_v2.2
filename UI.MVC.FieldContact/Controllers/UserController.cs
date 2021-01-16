using CustomFramework;
using CustomFramework.SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UI.MVC.FieldContact.Models;
//using UI.MVC.FieldContact.CustomFramework;
//using UI.MVC.FieldContact.CustomFramework.SecurityManager;
using UI.MVC.FieldContact.PageModel.Management;
using UI.MVC.FieldContact.PageModel.User;

namespace UI.MVC.FieldContact.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            try
            {
                //using (DataService ds = new DataService())
                //{
                //    ds.Context.Membership_User.FirstOrDefault(x => x.ID == 5).EmployeeDetail = new EmployeeDetail
                //    {
                //        Name="Muzaffer",
                //        LastName="Yavuz",
                //         BirthDate=Convert.ToDateTime("26.03.1990"),
                //          Salary=2750,
                //           HireDate=Convert.ToDateTime("18.07.2016"),
                //            PersonelPhone="05348308493",
                             
                //    };

                //    ds.Context.SaveChanges();
                //}
            }
            catch (Exception ex)
            {
                
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            if (CustomSecurity.LogIn(userName,password))
            {
                return RedirectToAction("Sum", "Home");
            }

            return View();
        }

        [HttpGet]
        public JsonResult UserInfo(UserLayoutModel model)
        {
            try
            {
                using (DataService db = new DataService())
                {
                    var cookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    var gCode = Guid.Parse(ticket.UserData);
                    var member = db.Context.Membership_User.FirstOrDefault(x => x.UserCode == gCode);
                    var user = db.Context.EmployeeDetails.FirstOrDefault(x => x.ID == member.ID);
                    model.UserName = user.Name + " " + user.LastName;
                }
            }
            catch (Exception ex)
            {

            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ForgotPassword(string mailAddress)
        {
            var resultModel = new JsonResultModel<string>();
            try
            {
                using (DataService db = new DataService())
                {
                    var user = db.Context.Membership_User.FirstOrDefault(x => x.Username == mailAddress);
                    if (!(user == null))
                    {
                        SmtpClient SmtpServer = new SmtpClient("smtp.live.com");
                        var mail = new MailMessage();
                        mail.From = new MailAddress("zaffer_yavuz@hotmail.com");
                        mail.To.Add(mailAddress);
                        mail.IsBodyHtml = true;
                        mail.Subject = "Sifre Sıfırlama Kodu";
                        mail.Body = string.Format("Aşağıdaki şifreyi kopyalayıp, kullanıcı giriş ekranındaki " +
                            "şifre bölümüne yapıştırın ve Giriş butonuna basınız. " +
                            "Ardından yeni şifrenizi girin.<br> Şifre:{0} ", user.Password);
                        SmtpServer.Port = 587;
                        SmtpServer.UseDefaultCredentials = false;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("zaffer_yavuz@hotmail.com", "123besiktas");
                        SmtpServer.EnableSsl = true;
                        try
                        {
                            SmtpServer.Send(mail);
                            resultModel.Status = JsonResultType.Success;
                            resultModel.Message = "Şifre sıfırlama maili gönderildi.";
                        }
                        catch (Exception)
                        {
                            resultModel.Message = "Şifre sıfırlama maili gönderilemedi.";
                            resultModel.Status = JsonResultType.Error;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                resultModel.Message = "Hata oluştu";
                resultModel.Status = JsonResultType.Error;
            }

            return Json(resultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            //HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName]
            var cookie=HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Login","User");
        }
    }
}