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
            var bookEditViewModel = new BookEditViewModel()
            {
                Authors = PopulateAuthorsDropdown()
            };
            return View(bookEditViewModel);
        }

        [HttpPost]
        public ActionResult Create(BookEditViewModel bookToCreate)
        {
            if (ModelState.IsValid)
            {
                var book = new Book()
                {
                    AuthorId = bookToCreate.AuthorId,
                    Genre = bookToCreate.Genre,
                    Title = bookToCreate.Title,
                };
                repository.CreateBook(book);
                ViewBag.Message = "Book created successfully";
                return RedirectToAction("Index");
            }
            bookToCreate.Authors = PopulateAuthorsDropdown();
            return View(bookToCreate);
        }

        public ViewResult Delete(int id)
        {
            repository.Delete(id);
            ViewBag.Message = "Book deleted successfully";
            return View("Index", repository.GetAllBooks());
        }


        public ActionResult Edit(int id)
        {
            var dbBook = repository.GetBook(id);

            if (dbBook == null)
            {
                return HttpNotFound("No book found for id " + id);
            }

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
            book.Authors = PopulateAuthorsDropdown(book.AuthorId);
            return View(book);
        }


        private List<SelectListItem> PopulateAuthorsDropdown(int selectedAuthorId = -1)
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

        public ViewResult Details(int id)
        {
            var book = repository.GetBook(id);
            return View(book);
        }
    }
}