using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;


namespace CustomFramework.SecurityManager
{
    public class CustomSecurity
    {
        public static bool UserIsInRole(string controller, string action)
        {
          
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            
            if (cookie==null)
            {
                return false;
            }
            //var cookie = HttpContext.Current.Request.Cookies[0];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var gCode = Guid.Parse(ticket.UserData);

            cookie.Expires = DateTime.Now.AddMinutes(58);

            using (DataService dataService = new DataService())
            {
                //bool trueMu= dataService.Context.Membership_User.Any(x => x.UserCode == gCode &&( x.Membership_Role.Any(y => y.Application_Action.Any(z => z.Name == action && z.Application_Controller.Name == controller))|| x.Application_Action.Any(z=>z.Name==action&& z.Application_Controller.Name==controller)));
                //bool mycorrection = dataService.Context.Membership_User.Any(x => x.UserCode == gCode && x.Membership_Role.Any(y=>y.));
                return dataService.Context.Membership_User.Any(x => x.UserCode == gCode && (x.Membership_Role.Any(y => y.Application_Action.Any(z => z.Name == action && z.Application_Controller.Name == controller)) || x.Application_Action.Any(z => z.Name == action && z.Application_Controller.Name == controller)));
            }
        }

        public static bool LogIn(string username, string password)
        {
            using (DataService dataService = new DataService())
            {
                var user = dataService.Context.Membership_User.SingleOrDefault(x => x.Username == username);
                if (!(user==null))
                {
                    if (user.Password == null)
                    {
                        user.Password = StringCipher.Encrypt(password, user.Salt);
                        dataService.Context.SaveChanges();
                        return true;
                    }
                    if (password == user.Password)
                    {
                        user.Password = null;
                        dataService.Context.SaveChanges();
                        return false;
                    }
                    if (password == StringCipher.Decrypt(user.Password, user.Salt))
                    {
                        var ticket = new FormsAuthenticationTicket(1, user.Username, DateTime.Now, DateTime.Now.AddMinutes(58), true, user.UserCode.ToString());
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
                else
                {
                    return false;
                }
                
                //if (password == StringCipher.Decrypt(user.Password,user.Salt))
                //var saltedCode = StringCipher.Encrypt(password, user.Salt);

              
            }
        }
    }
}
