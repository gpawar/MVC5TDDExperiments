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
        }

        [TestMethod]
        public void CreateBookReturnsCreatedBook()
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
            var createdBook = bookToCreate;
            Mock.Arrange(() => repository.CreateBook(bookToCreate)).Returns(createdBook).MustBeCalled();

            //Act
            var controller = new HomeController(repository);
            ViewResult result = controller.Create(bookToCreate);
            var model = result.Model as Book;

            //Assert
            Assert.AreEqual(bookToCreate.Title, model.Title);
            Assert.AreEqual(bookToCreate.BookId, model.BookId);
            Assert.AreEqual(bookToCreate.Genre, model.Genre);
            Assert.AreEqual(bookToCreate.Author, model.Author);
            Assert.AreEqual("Book created successfully", result.ViewBag.Message);
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
