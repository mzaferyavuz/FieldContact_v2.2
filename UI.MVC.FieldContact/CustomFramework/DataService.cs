using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.CustomFramework
{
    public class DataService : IDisposable
    {
        private DataContext _context;
        public DataContext Context { get { return _context = _context ?? new DataContext(); } }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}