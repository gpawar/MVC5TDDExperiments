using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using MVC5TDDExperiments.Models;

namespace MVC5TDDExperiments.Tests.Helpers
{
    public static class BookHelper
    {
        private const int NO_ID = -1;

        public static Book CleanCode(int bookId = NO_ID, int authorId = NO_ID)
        {
            return CreateBook("Clean Code", "Programming", bookId, authorId);
        }

        public static Book CleanCode(int bookId)
        {
            return CreateBook(bookId, CleanCode().Title, CleanCode().Genre, AuthorHelper.RobertMartin());
        }

        public static Book ArtOfUnitTesting(int bookId = NO_ID, int authorId = NO_ID)
        {
            return CreateBook("The art of Unit Testing", "Programming", bookId, authorId);
        }

        public static Book ArtOfUnitTesting(int bookId)
        {
            return CreateBook(bookId, ArtOfUnitTesting().Title, ArtOfUnitTesting().Genre, AuthorHelper.RoyOsherove());
        }

        public static Book LordOfTheRings(int bookId = NO_ID, int authorId = NO_ID)
        {
            return CreateBook("The Lord of the Rings", "Adventure", bookId, authorId);
        }

        public static Book LordOfTheRings(int bookId)
        {
            return CreateBook(bookId, LordOfTheRings().Title, LordOfTheRings().Genre, AuthorHelper.JRRTolkien());
        }

        public static Book BilboTheHobbit(int bookId = NO_ID, int authorId = NO_ID)
        {
            return CreateBook("Bilbo the hobbit", "Adventure", bookId, authorId);
        }

        public static Book BilboTheHobbit(int bookId)
        {
            return CreateBook(bookId, BilboTheHobbit().Title, BilboTheHobbit().Genre, AuthorHelper.JRRTolkien());
        }

        public static Book CreateBook(string title, string genre, int bookId = NO_ID, int authorId = NO_ID)
        {
            if (bookId == NO_ID && authorId == NO_ID)
            {
                return new Book() { Title = title, Genre = genre };
            }

            if (authorId == NO_ID)
            {
                return new Book() { BookId = bookId, Title = title, Genre = genre };
            }

            return new Book() { BookId = bookId, AuthorId = authorId, Title = title, Genre = genre };
        }

        public static Book CreateBook(int bookId, string title, string genre, Author author)
        {
            if (author.AuthorId == NO_ID)
            {
                return new Book() { BookId = bookId, Title = title, Genre = genre, Author = author};
            }

            return new Book() { BookId = bookId, Author = author, AuthorId = author.AuthorId, Title = title, Genre = genre };
        }
    }
}
