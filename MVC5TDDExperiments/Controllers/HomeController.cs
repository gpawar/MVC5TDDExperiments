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
        public ActionResult Create(BookEditViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                var book = new Book(bookViewModel);
                repository.CreateBook(book);
                TempData["Message"] = "Book created successfully";
                return RedirectToAction("Index");
            }
            bookViewModel.Authors = PopulateAuthorsDropdown();
            return View(bookViewModel);
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

            var bookViewModel = new BookEditViewModel(dbBook)
            {
                Authors = PopulateAuthorsDropdown(dbBook.AuthorId),
            };

            return View(bookViewModel);
        }
        
        [HttpPost]
        public ActionResult Edit(BookEditViewModel bookViewModel)
        {
            if (!ModelState.IsValid)
            {
                bookViewModel.Authors = PopulateAuthorsDropdown(bookViewModel.AuthorId);
                return View(bookViewModel);
            }

            var bookEntity = new Book(bookViewModel);
            if (!TryValidateModel(bookEntity))
            {
                bookViewModel.Authors = PopulateAuthorsDropdown(bookViewModel.AuthorId);
                return View(bookViewModel);
            }

            try
            {
                repository.Save(bookEntity);
                TempData["Message"] = "Book edited successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                bookViewModel.Authors = PopulateAuthorsDropdown(bookViewModel.AuthorId);
                return View(bookViewModel);
            } 
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