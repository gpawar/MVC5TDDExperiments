using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC5TDDExperiments.Models
{
    public interface IRepository
    {
        List<Book> GetAllBooks();
        List<Author> GetAllAuthors();

        void CreateBook(Book bookToCreate);

        void Delete(int idToDelete);

        Book GetBook(int bookId);

        Author GetAuthor(int authorId);

        void Save(Book bookToEdit);

        void Dispose();
    }
}
