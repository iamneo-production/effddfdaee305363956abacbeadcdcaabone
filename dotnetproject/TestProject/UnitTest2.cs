using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using dotnetmvcapp.Controllers;
using dotnetmvcapp.Models;
using dotnetmvcapp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using Moq;

namespace dotnetmvcapp.Tests
{
    [TestFixture]
    public class BookControllerTests
    {
        private Mock<IBookService> mockBookService;
        private BookController controller;
        [SetUp]
        public void Setup()
        {
            mockBookService = new Mock<IBookService>();
            controller = new BookController(mockBookService.Object);
        }

        [Test]
        public void AddBook_ValidData_SuccessfulAddition_RedirectsToIndex()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            mockBookService.Setup(service => service.AddBook(It.IsAny<Book>())).Returns(true);
            var controller = new BookController(mockBookService.Object);
            var book = new Book(); // Provide valid Book data

            // Act
            var result = controller.AddBook(book) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
        [Test]
        public void AddBook_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            var controller = new BookController(mockBookService.Object);
            Book invalidBook = null; // Invalid Book data

            // Act
            var result = controller.AddBook(invalidBook) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid Book data", result.Value);
        }
        [Test]
        public void AddBook_FailedAddition_ReturnsViewWithModelError()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            mockBookService.Setup(service => service.AddBook(It.IsAny<Book>())).Returns(false);
            var controller = new BookController(mockBookService.Object);
            var book = new Book(); // Provide valid Book data

            // Act
            var result = controller.AddBook(book) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            // Check for expected model state error
            Assert.AreEqual("Failed to add the Book. Please try again.", controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }


        [Test]
        public void AddBook_Post_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            mockBookService.Setup(service => service.AddBook(It.IsAny<Book>())).Returns(true);
            var controller = new BookController(mockBookService.Object);
            var book = new Book();

            // Act
            var result = controller.AddBook(book) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public void AddBook_Post_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            var controller = new BookController(mockBookService.Object);
            controller.ModelState.AddModelError("error", "Error");
            var book = new Book();

            // Act
            var result = controller.AddBook(book) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(book, result.Model);
        }

        [Test]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            mockBookService.Setup(service => service.GetAllBooks()).Returns(new List<Book>());
            var controller = new BookController(mockBookService.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [Test]
        public void Search_ValidId_ReturnsViewResult()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            var expectedBook = new Book { Id = 1, Title = "BookTitle" };
            mockBookService.Setup(service => service.GetBookById(1)).Returns(expectedBook);
            var controller = new BookController(mockBookService.Object);

            // Act
            var result = controller.Search(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Search", result.ViewName);
            var model = result.Model as Book[];
            Assert.IsNotNull(model);
            Assert.AreEqual(expectedBook, model[0]);
        }

        [Test]
        public void Search_InvalidId_ReturnsViewResult()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            mockBookService.Setup(service => service.GetBookById(It.IsAny<int>())).Returns((Book)null);
            var controller = new BookController(mockBookService.Object);

            // Act
            var result = controller.Search(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Search", result.ViewName);
            var model = result.Model as Book[];
            Assert.IsNotNull(model);
            Assert.IsEmpty(model);
        }

        [Test]
        public void Delete_ValidId_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            mockBookService.Setup(service => service.DeleteBook(1)).Returns(true);
            var controller = new BookController(mockBookService.Object);

            // Act
            var result = controller.Delete(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public void Delete_InvalidId_ReturnsViewResult()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            mockBookService.Setup(service => service.DeleteBook(1)).Returns(false);
            var controller = new BookController(mockBookService.Object);

            // Act
            var result = controller.Delete(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }
    }
}
