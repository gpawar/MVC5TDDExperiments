﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
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
    public class EditHomeControllerTest
    {
        [TestMethod]
        public void EditWithoutIdReturnsBadRequest()
        {
            //Arrange

            //Act
            var controller = new HomeController();
            int? id = null;
            var result = controller.Edit(id) as HttpStatusCodeResult;

            //Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void EditViewModelValidationErrorRepopulatesAuthorsDropdown()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            int bookId = 1;
            int authorId = 3;
            string errorMessage = "The minimal length for the title is 3";
            var book = new BookEditViewModel()
            {
                AuthorId = AuthorHelper.RobertMartin(authorId).AuthorId,
                BookId = BookHelper.CleanCode(bookId, authorId).BookId,
                Title = "12", //Too short, min is 3 (defined in the view model)
                Genre = BookHelper.CleanCode(bookId, authorId).Genre,
            };
            Mock.Arrange(() => repository.Save((Book)Arg.AnyObject)).OccursNever();
            var authorsList = new List<Author>() { AuthorHelper.RobertMartin(1), AuthorHelper.JRRTolkien(authorId) };
            Mock.Arrange(() => repository.GetAllAuthors()).Returns(authorsList).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            controller.ModelState.AddModelError("Title", errorMessage); //add the view model validation error
            controller.ControllerContext = Mock.Create<ControllerContext>(); //needed by TryValidateModel(entity)
            var result = controller.Edit(book) as ViewResult;

            //Assert
            StandardAssertsForEditErrors(controller, result, book, errorMessage);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void EditDBExceptionRepopulatesAuthorsDropdown()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            int bookId = 1;
            int authorId = 1;
            var exceptionMessage = "test exception with message";
            var book = new BookEditViewModel()
            {
                AuthorId = AuthorHelper.RobertMartin(authorId).AuthorId,
                BookId = BookHelper.CleanCode(bookId, authorId).BookId,
                Title = BookHelper.CleanCode(bookId, authorId).Title,
                Genre = BookHelper.CleanCode(bookId, authorId).Genre,
            };
            Mock.Arrange(() => repository.Save((Book)Arg.AnyObject)).Throws(new Exception(exceptionMessage)).OccursOnce();
            var authorsList = new List<Author>() { AuthorHelper.RobertMartin(1), AuthorHelper.JRRTolkien(authorId) };
            Mock.Arrange(() => repository.GetAllAuthors()).Returns(authorsList).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            controller.ControllerContext = Mock.Create<ControllerContext>(); //needed by TryValidateModel(entity)
            var result = controller.Edit(book) as ViewResult;
            

            //Assert
            StandardAssertsForEditErrors(controller, result, book, exceptionMessage);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void EditEntityValidationErrorRepopulatesAuthorsDropdown()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            int bookId = 1;
            int authorId = 3;
            string errorMessage = "The minimal length for the title is 4 - From Entity";
            var book = new BookEditViewModel()
            {
                AuthorId = AuthorHelper.RobertMartin(authorId).AuthorId,
                BookId = BookHelper.CleanCode(bookId, authorId).BookId,
                Title = "123", //Too short, min is 4 (defined in the entity)
                Genre = BookHelper.CleanCode(bookId, authorId).Genre,
            };
            Mock.Arrange(() => repository.Save((Book)Arg.AnyObject)).OccursNever();
            var authorsList = new List<Author>() { AuthorHelper.RobertMartin(1), AuthorHelper.JRRTolkien(authorId) };
            Mock.Arrange(() => repository.GetAllAuthors()).Returns(authorsList).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            controller.ControllerContext = Mock.Create<ControllerContext>(); //needed by TryValidateModel(entity)
            var result = controller.Edit(book) as ViewResult;

            //Assert
            StandardAssertsForEditErrors(controller, result, book, errorMessage);
            Mock.Assert(repository);
        }

        private static void StandardAssertsForEditErrors(HomeController controller, ViewResult result, BookEditViewModel bookViewModel, string errorMessage)
        {
            var model = result.Model as BookEditViewModel;
            var errorStates = from m in controller.ModelState.Values select m.Errors;
            var selectedAuthor = model.Authors.Find(a => a.Selected);

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(errorMessage, errorStates.First().First().ErrorMessage);
            Assert.AreEqual(bookViewModel.AuthorId.ToString(), selectedAuthor.Value);
            Assert.IsNull(controller.ViewBag.Message);
            Assert.IsNull(controller.TempData["Message"]);
        }

        [TestMethod]
        public void EditIndexInvalidIdReturnsHttpNotFound()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            int unexistingBookId = 999;
            Book nullBook = null;
            Mock.Arrange(() => repository.GetBook(Arg.AnyInt)).Returns(nullBook).OccursOnce();
            Mock.Arrange(() => repository.GetAllAuthors()).OccursNever();

            //Act
            var controller = new HomeController(repository);
            var result = controller.Edit(unexistingBookId) as HttpNotFoundResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("No book found for id " + unexistingBookId, result.StatusDescription);
            Assert.AreEqual("404", result.StatusCode.ToString());
            Mock.Assert(repository);
        }
        
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
            controller.ControllerContext = Mock.Create<ControllerContext>(); //needed by TryValidateModel(entity)
            var result = controller.Edit(submittedBookViewModel) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Book edited successfully", controller.TempData["Message"]);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void EditIndexShowsBookEditFormWithBookContentWithPopulatedDropDownList()
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
            var result = controller.Edit(bookToEdit.BookId) as ViewResult;
            var model = result.Model as BookEditViewModel;
            var selectedItem = model.Authors.Find(b => Int32.Parse(b.Value) == bookToEdit.AuthorId);

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(model.BookId, bookToEdit.BookId);
            Assert.AreEqual(model.AuthorId, bookToEdit.AuthorId);
            Assert.AreEqual(model.Genre, bookToEdit.Genre);
            Assert.AreEqual(model.Title, bookToEdit.Title);
            Assert.IsTrue(selectedItem.Selected);
            Assert.IsNull(result.ViewBag.Message);
            Mock.Assert(repository);
        }
    }
}
