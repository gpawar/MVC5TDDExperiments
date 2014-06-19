using System.Collections.Generic;
using MVC5TDDExperiments.Models;

namespace MVC5TDDExperiments.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MVC5TDDExperiments.Models.BookStoreDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "MVC5TDDExperiments.Models.BookStoreDb";
        }

        protected override void Seed(MVC5TDDExperiments.Models.BookStoreDb context)
        {
            context.Books.AddOrUpdate(b => b.Title, 
                new Book() { Author = "Roy Osherove", Genre = "Programming", Title = "The art of Unit Testing" },
                new Book() { Author = "Robert C. Martin", Genre = "Programming", Title = "Clean Code" },
                new Book() { Author = "J. R. R. Tolkien", Genre = "Adventure", Title = "The Lord of the Rings" },
                new Book() { Author = "J. R. R. Tolkien", Genre = "Adventure", Title = "Bilbo the hobbit" });
        }
    }
}
