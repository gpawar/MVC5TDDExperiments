﻿using System;
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
            var createdBook = repository.CreateBook(bookToCreate);
            ViewBag.Message = "Book created successfully";
            return View(createdBook);
        }
    }
}