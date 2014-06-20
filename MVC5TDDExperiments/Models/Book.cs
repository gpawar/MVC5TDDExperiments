using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        public int BookId { get; set; }

        [StringLength(100)]
        [MinLength(4, ErrorMessage = "The minimal length for the title is 4 - From Entity")]
        public string Title { get; set; }

        [StringLength(100)]
        [MinLength(4, ErrorMessage = "The minimal length for the title is 4 - From Entity")]
        public string Genre { get; set; }

        [Required]
        public int AuthorId { get; set; }
        public virtual Author Author { get; set; }
    }
}