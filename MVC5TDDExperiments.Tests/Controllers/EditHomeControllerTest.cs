using System;
using System.Collections.Generic;
using System.Linq;
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
        public void EditViewModelValidationErrorRepopulatesAuthorsDropdown()
        {
            throw new NotImplementedException("todo: view model error");
        }

        [TestMethod]
        public void EditDBExceptionRepopulatesAuthorsDropdown()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            int bookId = 1;
            int authorId = 1;
            var book = new BookEditViewModel()
            {
                AuthorId = AuthorHelper.RobertMartin(authorId).AuthorId,
                BookId = BookHelper.CleanCode(bookId, authorId).BookId,
                Title = BookHelper.CleanCode(bookId, authorId).Title,
                Genre = BookHelper.CleanCode(bookId, authorId).Genre,
            };
            Mock.Arrange(() => repository.Save((Book) Arg.AnyObject)).Throws(new Exception("test exception with message")).OccursOnce();
            var authorsList = new List<Author>() { AuthorHelper.RobertMartin(1), AuthorHelper.JRRTolkien(authorId) };
            Mock.Arrange(() => repository.GetAllAuthors()).Returns(authorsList).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            controller.ControllerContext = Mock.Create<ControllerContext>(); //needed by TryValidateModel(entity)

            var result = controller.Edit(book) as ViewResult;
            var model = result.Model as BookEditViewModel;
            var errorStates = from m in controller.ModelState.Values select m.Errors;
            var selectedAuthor = model.Authors.Find(a => a.Selected);

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("test exception with message", errorStates.First().First().ErrorMessage);
            Assert.AreEqual(book.AuthorId.ToString(), selectedAuthor.Value);
            Assert.IsNull(controller.ViewBag.Message);
            Assert.IsNull(controller.TempData["Message"]);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void EditEntityValidationErrorRepopulatesAuthorsDropdown()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            int bookId = 1;
            int authorId = 3;
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
            var model = result.Model as BookEditViewModel;
            var errorStates = from m in controller.ModelState.Values select m.Errors;
            var selectedAuthor = model.Authors.Find(a => a.Selected);

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("The minimal length for the title is 4 - From Entity", errorStates.First().First().ErrorMessage);
            Assert.AreEqual(book.AuthorId.ToString(), selectedAuthor.Value);
            Assert.IsNull(controller.ViewBag.Message);
            Mock.Assert(repository);
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
