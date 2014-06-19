using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC5TDDExperiments.Models;

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
            this.repository = new BookDBRepository();
        }

        public ViewResult Index()
        {
            var books = repository.GetAll();
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
            var booksByGenre = repository.GetAll().Where(b => b.Genre == genre);
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
            return View("Index", repository.GetAll());
        }

        public ViewResult Delete(int id)
        {
            repository.Delete(id);
            ViewBag.Message = "Book deleted successfully";
            return View("Index", repository.GetAll());
        }


        public ViewResult Edit(int id)
        {
            var book = repository.Get(id);
            return View(book);
        }

        [HttpPost]
        public ViewResult Edit(Book bookToEdit)
        {
            repository.Save(bookToEdit);
            ViewBag.Message = "Book edited successfully";
            return View("Index", repository.GetAll());
        }
    }
}