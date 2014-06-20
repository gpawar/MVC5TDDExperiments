using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5TDDExperiments.Models.ViewModels
{
    public class BookEditViewModel
    {
        public BookEditViewModel() { }
        
        public BookEditViewModel(Book bookModel)
        {
            BookId = bookModel.BookId;
            Title = bookModel.Title;
            Genre = bookModel.Genre;
            AuthorId = bookModel.AuthorId;
        }

        public int BookId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int AuthorId { get; set; }
        public List<SelectListItem> Authors { get; set; }
    }
}