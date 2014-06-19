using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVC5TDDExperiments;
using MVC5TDDExperiments.Controllers;
using MVC5TDDExperiments.Models;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace MVC5TDDExperiments.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void EditFormSubmitReturnsToIndex()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            var bookToEdit = new Book() { BookId = 2, Author = "Robert C. Martin", Genre = "Programming", Title = "Clean Code" };
            Mock.Arrange(() => repository.Save(bookToEdit)).OccursOnce();
            Mock.Arrange(() => repository.GetAll()).Returns(new List<Book>() { bookToEdit }).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.Edit(bookToEdit);
            var model = result.Model as List<Book>;
            var insertedBook = model.Find(b => b.BookId == bookToEdit.BookId);

            //Assert
            Assert.AreEqual("Index", result.ViewName);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(insertedBook.BookId, bookToEdit.BookId);
            Assert.AreEqual(insertedBook.Author, bookToEdit.Author);
            Assert.AreEqual(insertedBook.Genre, bookToEdit.Genre);
            Assert.AreEqual(insertedBook.Title, bookToEdit.Title);
            Assert.AreEqual("Book edited successfully", result.ViewBag.Message);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void EditIndexShowsBookEditFormWithBookContent()
        {
            //Arrange
            var repository = Mock.Create<IRepository>();
            var bookToEdit = new Book() { BookId = 2, Author = "Robert C. Martin", Genre = "Programming", Title = "Clean Code" };
            Mock.Arrange(() => repository.Get(bookToEdit.BookId)).Returns(bookToEdit).OccursOnce();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.Edit(bookToEdit.BookId);
            var model = result.Model as Book;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(model.BookId, bookToEdit.BookId);
            Assert.AreEqual(model.Author, bookToEdit.Author);
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
            Mock.Arrange(() => repository.GetAll()).Returns(new List<Book>()
            {
                new Book() {BookId = 2, Author = "Robert C. Martin", Genre = "Programming", Title = "Clean Code"}
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
            var bookToCreate = new Book()
            {
                BookId = 10,
                Title = "Bilbo the hobbit",
                Author = "J. R. R. Tolkien",
                Genre = "Adventure"
            };
            Mock.Arrange(() => repository.CreateBook(bookToCreate)).OccursOnce();
            Mock.Arrange(() => repository.GetAll()).Returns(new List<Book>()
            {
                bookToCreate,
                new Book() {BookId = 1, Author = "Roy Osherove", Genre = "Programming", Title = "The art of Unit Testing"}
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
            Mock.Arrange(() => repository.GetAll()).Returns(new List<Book>()
            {
                new Book() {BookId = 1, Author = "Roy Osherove", Genre = "Programming", Title = "The art of Unit Testing"},
                new Book() {BookId = 2, Author = "Robert C. Martin", Genre = "Programming", Title = "Clean Code"},
                new Book() {BookId = 3, Author = "J. R. R. Tolkien", Genre = "Adventure", Title = "The Lord of the Rings"}
            }).MustBeCalled();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.FindByGenre("Programming");
            var model = result.Model as IEnumerable<Book>;

            //Assert
            Assert.AreEqual(2, model.Count());
            Assert.AreEqual("Roy Osherove", model.ToList()[0].Author);
            Assert.AreEqual("Robert C. Martin", model.ToList()[1].Author);
            Assert.AreEqual("The art of Unit Testing", model.ToList()[0].Title);
            Assert.AreEqual("Clean Code", model.ToList()[1].Title);
            Mock.Assert(repository);
        }

        [TestMethod]
        public void IndexReturnsAllBooksInDB()
        {
            //Arrange: simulate the access to the DB with a Mock
            //See JustMock documentation: http://www.telerik.com/help/justmock/basic-usage-arrange-act-assert.html
            var repository = Mock.Create<IRepository>();
            Mock.Arrange(() => repository.GetAll()).Returns(new List<Book>()
            {
                new Book() {BookId = 1, Author = "Roy Osherove", Genre = "Programming", Title = "The art of Unit Testing"},
                new Book() {BookId = 2, Author = "Robert C. Martin", Genre = "Programming", Title = "Clean Code"}
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
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
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
