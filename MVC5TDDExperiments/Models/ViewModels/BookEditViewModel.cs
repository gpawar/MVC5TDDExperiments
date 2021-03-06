﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [MinLength(3, ErrorMessageResourceType = typeof(MVC5TDDExperiments.Resources.Messages), ErrorMessageResourceName = "ErrorMessageLength3")]
        [MaxLength(100)]
        public string Title { get; set; }

        [MinLength(3, ErrorMessage = "The minimal length for the genre is 3")]
        [MaxLength(100)]
        public string Genre { get; set; }

        [DisplayName("Author")]
        public int AuthorId { get; set; }
        public List<SelectListItem> Authors { get; set; }
    }
}