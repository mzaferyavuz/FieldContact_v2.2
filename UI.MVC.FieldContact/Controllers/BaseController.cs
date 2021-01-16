using CustomFramework;
using CustomFramework.SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using UI.MVC.FieldContact.PageModel.User;

namespace UI.MVC.FieldContact.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.RouteData.Values["controller"].ToString();
            var action = filterContext.RouteData.Values["action"].ToString();

            if (CustomSecurity.UserIsInRole(controller, action))
            {


            }
            else
            {
                var basecookie=System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                    

                
                if (!(basecookie == null))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "Home" }, { "action", "Sum" }
                });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "User" }, { "action", "Login" }
                });
                }
                //if (controller == "Home" && action == "Sum")
                //{
                //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                //    { "controller", "User" }, { "action", "Login" }
                //});
                //}
                //else
                //{
                //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                //    { "controller", "Home" }, { "action", "Sum" }
                //});
                //}

            }

            base.OnActionExecuting(filterContext);
        }
    }
}