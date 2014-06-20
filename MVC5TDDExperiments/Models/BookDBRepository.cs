using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.ModelBinding;
using Microsoft.Owin.Security.Provider;

namespace MVC5TDDExperiments.Models
{
    class BookDBRepository : IRepository
    {
        private BookStoreDb db = new BookStoreDb();
        public BookStoreDb BookStoreDb
        {
            get { return db; }
        }

        public List<Book> GetAllBooks()
        {
            return db.Books.Include(b => b.Author).ToList();
        }

        public List<Author> GetAllAuthors()
        {
            return db.Authors.ToList();
        }

        public void CreateBook(Book bookToCreate)
        {
            db.Books.Add(bookToCreate);
            db.SaveChanges();
        }

        public void Delete(int idToDelete)
        {
            var book = db.Books.Find(idToDelete);
            db.Books.Remove(book);
            db.SaveChanges();
        }

        public Book GetBook(int bookId)
        {
            return db.Books.Find(bookId);
        }

        public Author GetAuthor(int authorId)
        {
            return db.Authors.Find(authorId);
        }

        public void Save(Book bookToEdit)
        {
            db.Entry(bookToEdit).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
