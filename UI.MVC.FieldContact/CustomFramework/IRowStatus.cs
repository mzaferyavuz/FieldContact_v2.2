using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.CustomFramework
{
    public interface IRowStatus : IEntity
    {
        Nullable<int> RowStatusID { get; set; }
    }
}