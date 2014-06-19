using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC5TDDExperiments.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public Author Author { get; set; }
    }
}