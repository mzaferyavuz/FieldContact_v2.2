using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace UI.MVC.FieldContact.CustomFramework.SecurityManager
{
    public class CustomSecurity
    {
        public static bool UserIsInRole(string controller, string action)
        {

            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null)
            {
                return false;
            }
            //var cookie = HttpContext.Current.Request.Cookies[0];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var gCode = Guid.Parse(ticket.UserData);

            using (DataService dataService = new DataService())
            {
                return dataService.Context.Membership_User.Any(x => x.UserCode == gCode && x.Membership_Role.Any(y => y.Application_Action.Any(z => z.Name == action && z.Application_Controller.Name == controller)));
            }
        }

        public static bool LogIn(string username, string password)
        {
            using (DataService dataService = new DataService())
            {
                var user = dataService.Context.Membership_User.SingleOrDefault(x => x.Username == username);

                //if (password == StringCipher.Decrypt(user.Password,user.Salt))
                if (user.Password == StringCipher.Encrypt(password, user.Salt))
                {
                    var ticket = new FormsAuthenticationTicket(1, user.Username, DateTime.Now, DateTime.Now.AddMinutes(-1), true, user.UserCode.ToString());
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);

                    if (ticket.IsPersistent)
                    {
                        cookie.Expires = ticket.Expiration;
                    }

                    HttpContext.Current.Response.Cookies.Add(cookie);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}