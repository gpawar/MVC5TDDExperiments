using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC5TDDExperiments.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName {
            get { return FirstName + " " + LastName; }
        }

        public ICollection<Book> Books  { get; set; }
    }
}