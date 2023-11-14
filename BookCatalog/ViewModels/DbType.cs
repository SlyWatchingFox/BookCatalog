using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.ViewModels
{
    public class DbType
    {
        public string Title { get; set; }
        public DbType(string titel) 
        {
            Title = titel;
        }
    }
}
