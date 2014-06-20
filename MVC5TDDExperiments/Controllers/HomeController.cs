using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC5TDDExperiments.Models;
using MVC5TDDExperiments.Models.ViewModels;

namespace MVC5TDDExperiments.Controllers
{
    public class HomeController : Controller
    {
        private IRepository repository; 

        public HomeController(IRepository repository)
        {
            this.repository = repository;
        }

        public HomeController()
        {
            this.repository = new BookStoreRepository();
        }

        public ViewResult Index()
        {
            var books = repository.GetAllBooks();
            return View(books);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ViewResult FindByGenre(string genre)
        {
            var booksByGenre = repository.GetBooksByGenre(genre);
            return View(booksByGenre);
        }

        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Create(Book bookToCreate)
        {
            repository.CreateBook(bookToCreate);
            ViewBag.Message = "Book created successfully";
            return View("Index", repository.GetAllBooks());
        }

        public ViewResult Delete(int id)
        {
            repository.Delete(id);
            ViewBag.Message = "Book deleted successfully";
            return View("Index", repository.GetAllBooks());
        }


        public ViewResult Edit(int id)
        {
            var dbBook = repository.GetBook(id);
            var bookViewModel = new BookEditViewModel()
            {
                BookId = dbBook.BookId,
                AuthorId = dbBook.AuthorId,
                Genre = dbBook.Genre,
                Title = dbBook.Title,
                Authors = PopulateAuthorsDropdown(dbBook.AuthorId),
            };

            return View(bookViewModel);
        }
        
        [HttpPost]
        public ActionResult Edit(BookEditViewModel book)
        {
            if (ModelState.IsValid)
            {
                var bookEntity = new Book()
                {
                    BookId = book.BookId,
                    Genre = book.Genre,
                    Title = book.Title,
                    AuthorId = book.AuthorId
                };
                repository.Save(bookEntity);
                ViewBag.Message = "Book edited successfully";
                return RedirectToAction("Index");
            }
            return View(book);
        }


        private List<SelectListItem> PopulateAuthorsDropdown(int selectedAuthorId)
        {
            var authors = repository.GetAllAuthors();

            var authorsItems = new List<SelectListItem>();
            foreach (var author in authors)
            {
                var item = new SelectListItem() { Text = author.FullName, Value = author.AuthorId.ToString() };
                if (author.AuthorId == selectedAuthorId)
                {
                    item.Selected = true;
                }
                authorsItems.Add(item);
            }

            return authorsItems;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}