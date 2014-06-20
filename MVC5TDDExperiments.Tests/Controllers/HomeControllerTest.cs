﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVC5TDDExperiments;
using MVC5TDDExperiments.Controllers;
using MVC5TDDExperiments.Models;
using MVC5TDDExperiments.Models.ViewModels;
using MVC5TDDExperiments.Tests.Helpers;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace MVC5TDDExperiments.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        /**
         * Tests todo:
         *  - error on model state in edit
         *  - populate authors in view model also on error
         *  - test for HomeController.PopulateAuthorsDropdown
         */

        [TestMethod]
        public void EditFormSubmitReturnsToIndex()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            var submittedBookViewModel = new BookEditViewModel()
            {
                BookId = 2,
                AuthorId = 1,
                Genre = BookHelper.CleanCode().Genre,
                Title = BookHelper.CleanCode().Title,
                Authors = null //after post submit this value is null
            };
            Mock.Arrange(() => repository.Save((Book) Arg.AnyObject)).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            var result = controller.Edit(submittedBookViewModel) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Book edited successfully", controller.ViewBag.Message);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void EditIndexShowsBookEditFormWithBookContent()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            var bookToEdit = BookHelper.CleanCode(bookId: 2, authorId: 1);
            Mock.Arrange(() => repository.GetBook(bookToEdit.BookId)).Returns(bookToEdit).OccursOnce();
            //called to populate the dropdownlist
            Mock.Arrange(() => repository.GetAllAuthors()).Returns(
                new List<Author>()
                {
                    AuthorHelper.RobertMartin(1),
                    AuthorHelper.RoyOsherove(2),
                }).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.Edit(bookToEdit.BookId);
            var model = result.Model as BookEditViewModel;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(model.BookId, bookToEdit.BookId);
            Assert.AreEqual(model.AuthorId, bookToEdit.AuthorId);
            Assert.AreEqual(model.Genre, bookToEdit.Genre);
            Assert.AreEqual(model.Title, bookToEdit.Title);
            Assert.IsNull(result.ViewBag.Message);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void DeleteBookReturnsToIndex()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            const int bookIdToDelete = 1;
            Mock.Arrange(() => repository.GetAllBooks()).Returns(new List<Book>()
            {
                BookHelper.CleanCode(2)
            }).OccursOnce();
            Mock.Arrange(() => repository.Delete(1)).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.Delete(bookIdToDelete);
            var resultViewName = result.ViewName;
            var model = result.Model as List<Book>;
            var insertedBook = model.Find(b => b.BookId == bookIdToDelete);

            //Assert
            Assert.AreEqual("Index", resultViewName);
            Assert.AreEqual(1, model.Count);
            Assert.IsInstanceOfType(model, typeof(List<Book>));
            Assert.IsNull(insertedBook);
            Assert.AreEqual("Book deleted successfully", result.ViewBag.Message);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void CreateIndexShowsEmptyCreationForm()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.Create();
            var model = result.Model;

            //Assert
            Assert.IsNull(model);
            Assert.IsNull(result.ViewBag.Message);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void CreateBookReturnsToIndex()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            var bookToCreate = BookHelper.BilboTheHobbit(10);
            Mock.Arrange(() => repository.CreateBook(bookToCreate)).OccursOnce();
            Mock.Arrange(() => repository.GetAllBooks()).Returns(new List<Book>()
            {
                bookToCreate,
                BookHelper.ArtOfUnitTesting(1)
            }).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.Create(bookToCreate);
            var resultViewName = result.ViewName;
            var model = result.Model as List<Book>;
            var insertedBook = model.Find(b => b.BookId == bookToCreate.BookId);

            //Assert
            Assert.AreEqual("Index", resultViewName);
            Assert.AreEqual(2, model.Count);
            Assert.IsInstanceOfType(model, typeof(List<Book>));
            Assert.AreEqual(bookToCreate.Author, insertedBook.Author);
            Assert.AreEqual(bookToCreate.Title, insertedBook.Title);
            Assert.AreEqual(bookToCreate.Genre, insertedBook.Genre);
            Assert.AreEqual("Book created successfully", result.ViewBag.Message);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void FindByGenreReturnsAllInGenre()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            Mock.Arrange(() => repository.GetBooksByGenre(BookHelper.ArtOfUnitTesting(1).Genre)).Returns(new List<Book>()
            {
                BookHelper.ArtOfUnitTesting(1),
                BookHelper.CleanCode(2)
            }).MustBeCalled();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.FindByGenre("Programming");
            var model = result.Model as IEnumerable<Book>;

            //Assert
            Assert.AreEqual(2, model.Count());
            Assert.AreEqual(AuthorHelper.RoyOsherove().FullName, model.ToList()[0].Author.FullName);
            Assert.AreEqual(AuthorHelper.RobertMartin().FullName, model.ToList()[1].Author.FullName);
            Assert.AreEqual(BookHelper.ArtOfUnitTesting().Title, model.ToList()[0].Title);
            Assert.AreEqual(BookHelper.CleanCode().Title, model.ToList()[1].Title);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void IndexReturnsAllBooksInDB()
        {
            //Arrange: simulate the access to the DB with a Mock
            //See JustMock documentation: http://www.telerik.com/help/justmock/basic-usage-arrange-act-assert.html
            var repository = Mock.Create<IRepository>();
            Mock.Arrange(() => repository.GetAllBooks()).Returns(new List<Book>()
            {
                BookHelper.ArtOfUnitTesting(1),
                BookHelper.CleanCode(2)
            }).MustBeCalled();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.Index();
            var model = result.Model as IEnumerable<Book>;

            //Assert
            Assert.AreEqual(2, model.Count());
            Mock.Assert(repository);

        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
