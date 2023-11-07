using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.ViewModels
{
    public class Book
    {
        public string Publisher { get; set; }
        public string Year { get;set; }  
        public string Author { get; set; }
        public string Name { get; set; }
        public Book(string publisher, string year, string author, string name)
        {
            Publisher = publisher;
            Year = year;
            Author = author;
            Name = name;
        }
    }
}
