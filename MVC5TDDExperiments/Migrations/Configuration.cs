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
            var roy = new Author() {FirstName = "Roy", LastName = "Osherove"};
            var robert = new Author() {FirstName = "Robert C.", LastName = "Martin"};
            var jrr = new Author() {FirstName = "J. R. R.", LastName = "Tolkien"};
            context.Authors.AddOrUpdate(a => a.FirstName, roy, robert, jrr);

            context.Books.AddOrUpdate(b => b.Title, 
                new Book() { Author = roy, Genre = "Programming", Title = "The art of Unit Testing" },
                new Book() { Author = robert, Genre = "Programming", Title = "Clean Code" },
                new Book() { Author = jrr, Genre = "Adventure", Title = "The Lord of the Rings" },
                new Book() { Author = jrr, Genre = "Adventure", Title = "Bilbo the hobbit" });

            context.SaveChanges();
        }
    }
}
