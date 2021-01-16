using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomFramework
{
    public interface IRowStatus : IEntity
    {
        Nullable<int> RowStatusID { get; set; }
    }
}
