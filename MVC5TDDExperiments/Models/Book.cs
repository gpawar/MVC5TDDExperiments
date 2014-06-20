using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC5TDDExperiments.Models.ViewModels;

namespace MVC5TDDExperiments.Models
{
    public class Book
    {
        public Book() { }
        
        public Book(BookEditViewModel bookViewModel)
        {
            BookId = bookViewModel.BookId;
            Title = bookViewModel.Title;
            Genre = bookViewModel.Genre;
            AuthorId = bookViewModel.AuthorId;
        }

        public int BookId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }

        public int AuthorId { get; set; }
        public virtual Author Author { get; set; }
    }
}