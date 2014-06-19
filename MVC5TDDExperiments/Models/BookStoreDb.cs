using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC5TDDExperiments.Models
{
    public class BookStoreDb : DbContext
    {
        public BookStoreDb() : base("name=DefaultConnection")
        {
            
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}