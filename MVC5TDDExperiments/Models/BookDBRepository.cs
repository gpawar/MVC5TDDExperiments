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
                new Book() { BookId = 4, Author = "J. R. R. Tolkien", Genre = "Adventure", Title = "Bilbo the hobbit"}
            };
        }

        public List<Book> GetAll()
        {
            return books;
        }

        public void CreateBook(Book bookToCreate)
        {
            int id = books.Max(b => b.BookId) + 1;
            bookToCreate.BookId = id;
            books.Add(bookToCreate);
        }


        public void Delete(int idToDelete)
        {
            var bookToDelete = books.Find(b => b.BookId == idToDelete);
            books.Remove(bookToDelete);
        }


        public Book Get(int bookId)
        {
            var book = books.Find(b => b.BookId == bookId);
            return book;
        }


        public void Save(Book bookToEdit)
        {
            var originalBook = books.Find(b => b.BookId == bookToEdit.BookId);
            originalBook.Author = bookToEdit.Author;
            originalBook.Title = bookToEdit.Title;
            originalBook.Genre = bookToEdit.Genre;
        }
    }
}
