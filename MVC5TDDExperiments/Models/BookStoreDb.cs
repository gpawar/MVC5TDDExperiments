using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MVC5TDDExperiments.Models
{
    public class BookStoreDb : IdentityDbContext<ApplicationUser>
    {
        public static BookStoreDb Create()
        {
            return new BookStoreDb();
        }

        public BookStoreDb() : base("DefaultConnection", throwIfV1Schema: false)
        {
            
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}