using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin.Security.Provider;

namespace MVC5TDDExperiments.Models
{
    class BookDBRepository : IRepository
    {
        //TODO: use a real DB
        private static List<Book> books;
        static BookDBRepository()
        {
            books = new List<Book>()
            {
                new Book() { BookId = 1, Author = "Roy Osherove", Genre = "Programming", Title = "The art of Unit Testing"},
                new Book() { BookId = 2, Author = "Robert C. Martin", Genre = "Programming", Title = "Clean Code"},
                new Book() { BookId = 3, Author = "J. R. R. Tolkien", Genre = "Adventure", Title = "The Lord of the Rings"},
                new Book() { BookId = 3, Author = "J. R. R. Tolkien", Genre = "Adventure", Title = "Bilbo the hobbit"}
            };
        }

        public List<Book> GetAll()
        {
            return books;
        }

        public void CreateBook(Book bookToCreate)
        {
            books.Add(bookToCreate);
        }
    }
}
