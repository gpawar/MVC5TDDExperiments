using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        [TestMethod]
        public void CreateWithDBExceptionAddsErrorToModelStateAndRepopulatesAuthorsDropdown()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            int bookId = 1;
            int authorId = 1;
            var exceptionMessage = "Unable to save changes. Try again, and if the problem persists, see your system administrator.";
            var book = new BookEditViewModel()
            {
                AuthorId = AuthorHelper.RobertMartin(authorId).AuthorId,
                BookId = BookHelper.CleanCode(bookId, authorId).BookId,
                Title = BookHelper.CleanCode(bookId, authorId).Title,
                Genre = BookHelper.CleanCode(bookId, authorId).Genre,
            };
            Mock.Arrange(() => repository.CreateBook((Book)Arg.AnyObject)).Throws(new Exception()).OccursOnce();
            var authorsList = new List<Author>() { AuthorHelper.RobertMartin(1), AuthorHelper.JRRTolkien(authorId) };
            Mock.Arrange(() => repository.GetAllAuthors()).Returns(authorsList).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            controller.ControllerContext = Mock.Create<ControllerContext>(); //needed by TryValidateModel(entity)
            var result = controller.Create(book) as ViewResult;
            var model = result.Model as BookEditViewModel;
            var errorStates = from m in controller.ModelState.Values select m.Errors;
            var selectedAuthor = model.Authors.Find(a => a.Selected);

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(exceptionMessage, errorStates.First().First().ErrorMessage);
            Assert.AreEqual(book.AuthorId.ToString(), selectedAuthor.Value);
            Assert.IsNull(controller.ViewBag.Message);
            Assert.IsNull(controller.TempData["Message"]);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void DetailsWithoutIdReturnsBadRequest()
        {
            //Arrange

            //Act
            var controller = new HomeController();
            int? id = null;
            var result = controller.Details(id) as HttpStatusCodeResult;

            //Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void DeleteWithoutIdReturnsBadRequest()
        {
            //Arrange

            //Act
            var controller = new HomeController();
            int? id = null;
            var result = controller.Delete(id) as HttpStatusCodeResult;

            //Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void DetailsShowsCompleteBookData()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            var cleanCode = BookHelper.CleanCode(1);
            Mock.Arrange(() => repository.GetBook(cleanCode.BookId)).Returns(cleanCode).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            var result = controller.Details(cleanCode.BookId) as ViewResult;
            var model = result.Model as Book;

            //Assert
            Assert.IsInstanceOfType(model, typeof(Book));
            Assert.AreEqual(model.BookId, cleanCode.BookId);
            Assert.AreEqual(model.AuthorId, cleanCode.AuthorId);
            Assert.AreEqual(model.Genre, cleanCode.Genre);
            Assert.AreEqual(model.Title, cleanCode.Title);
            Assert.AreEqual("", result.ViewName);
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
            ViewResult result = controller.Delete(bookIdToDelete) as ViewResult;
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
        public void CreateIndexShowsEmptyCreationFormWithPopulatedDropDownList()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            var authorsList = new List<Author>(){ AuthorHelper.RobertMartin(1), AuthorHelper.JRRTolkien(2) };
            Mock.Arrange(() => repository.GetAllAuthors()).Returns(authorsList).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.Create();
            var model = result.Model as BookEditViewModel;

            //Assert
            Assert.IsInstanceOfType(model, typeof(BookEditViewModel));
            Assert.AreEqual(2, model.Authors.Count);
            Assert.IsFalse(model.Authors.Exists(a => a.Selected));
            Assert.IsNull(result.ViewBag.Message);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void CreateBookReturnsToIndexWithMessage()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            var submittedBookViewModel = new BookEditViewModel()
            {
                Genre = BookHelper.BilboTheHobbit().Genre,
                Title = BookHelper.BilboTheHobbit().Title,
                AuthorId = 1,
                Authors = null //after post submit this value is null
            };
            Mock.Arrange(() => repository.CreateBook((Book) Arg.AnyObject)).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            var result = controller.Create(submittedBookViewModel) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Book created successfully", controller.TempData["Message"]);
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
