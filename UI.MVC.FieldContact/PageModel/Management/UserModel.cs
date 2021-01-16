using CustomFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.PageModel.Management
{
    public class UserModel:Membership_User
    {
        public string UserName { get; set; }
    }
}