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
            var roy = new Author() {AuthorId = 1, FirstName = "Roy", LastName = "Osherove"};
            var robert = new Author() { AuthorId = 2, FirstName = "Robert C.", LastName = "Martin" };
            var jrr = new Author() { AuthorId = 3, FirstName = "J. R. R.", LastName = "Tolkien" };
            context.Authors.AddOrUpdate(a => a.AuthorId, roy, robert, jrr);

            context.Books.AddOrUpdate(b => b.BookId, 
                new Book() { BookId = 1, AuthorId = roy.AuthorId, Genre = "Programming", Title = "The art of Unit Testing" },
                new Book() { BookId = 2, AuthorId = robert.AuthorId, Genre = "Programming", Title = "Clean Code" },
                new Book() { BookId = 3, AuthorId = jrr.AuthorId, Genre = "Adventure", Title = "The Lord of the Rings" },
                new Book() { BookId = 4, AuthorId = jrr.AuthorId, Genre = "Adventure", Title = "Bilbo the hobbit" });

            context.SaveChanges();

            SeedUserRoles(context);
        }

        private void SeedUserRoles(MVC5TDDExperiments.Models.BookStoreDb context)
        {
            //TODO: create default user
            
        }
    }
}
