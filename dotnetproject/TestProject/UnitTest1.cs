using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using dotnetapiapp.Controllers;
using dotnetapiapp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
namespace dotnetapiapp.Tests
{
    [TestFixture]
    public class BookControllerTests
    {
        private BookController _BookController;
        private BookStoreContext _context;

        [SetUp]
        public void Setup()
        {
            // Initialize an in-memory database for testing
            var options = new DbContextOptionsBuilder<BookStoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new BookStoreContext(options);
            _context.Database.EnsureCreated(); // Create the database

            // Seed the database with sample data
            _context.Books.AddRange(new List<Book>
            {
                new Book { Id = 1, Title = "Book 1",Author = "Author One",Price = 230,Quantity=10 },
                new Book { Id = 2, Title = "Book 2",Author = "Two Author",Price = 200,Quantity=20 },
                new Book { Id = 3, Title = "Book 3",Author = "Three Author",Price = 209,Quantity=30 },
            });
            _context.SaveChanges();

            _BookController = new BookController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted(); // Delete the in-memory database after each test
            _context.Dispose();
        }
        [Test]
        public void BookClassExists()
        {
            // Arrange
            Type BookType = typeof(Book);

            // Act & Assert
            Assert.IsNotNull(BookType, "Book class not found.");
        }
        [Test]
        public void Book_Properties_Title_ReturnExpectedDataTypes()
        {
            // Arrange
            Book book = new Book();
            PropertyInfo propertyInfo = book.GetType().GetProperty("Title");
            // Act & Assert
            Assert.IsNotNull(propertyInfo, "Title property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "Title property type is not string.");
        }
[Test]
        public void Book_Properties_Author_ReturnExpectedDataTypes()
        {
            // Arrange
            Book book = new Book();
            PropertyInfo propertyInfo = book.GetType().GetProperty("Author");
            // Act & Assert
            Assert.IsNotNull(propertyInfo, "Author property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "Author property type is not string.");
        }


        [Test]
        public async Task GetAllBooks_ReturnsOkResult()
        {
            // Act
            var result = await _BookController.GetAllBooks();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetAllBooks_ReturnsAllBooks()
        {
            // Act
            var result = await _BookController.GetAllBooks();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;

            Assert.IsInstanceOf<IEnumerable<Book>>(okResult.Value);
            var books = okResult.Value as IEnumerable<Book>;

            var BookCount = books.Count();
            Assert.AreEqual(3, BookCount); // Assuming you have 3 Books in the seeded data
        }

        [Test]
        public async Task GetBookById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var existingId = 1;

            // Act
            var result = await _BookController.GetBookById(existingId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetBookById_ExistingId_ReturnsBook()
        {
            // Arrange
            var existingId = 1;

            // Act
            var result = await _BookController.GetBookById(existingId);

            // Assert
            Assert.IsNotNull(result);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;

            var book = okResult.Value as Book;
            Assert.IsNotNull(book);
            Assert.AreEqual(existingId, book.Id);
        }

        [Test]
        public async Task GetBookById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingId = 99; // Assuming this ID does not exist in the seeded data

            // Act
            var result = await _BookController.GetBookById(nonExistingId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task AddBook_ValidData_ReturnsOkResult()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "New Book",Author = "New Author",Price = 2030,Quantity=20
            };

            // Act
            var result = await _BookController.AddBook(newBook);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }
        [Test]
        public async Task DeleteBook_ValidId_ReturnsNoContent()
        {
            // Arrange
              // var controller = new BooksController(context);

                // Act
                var result = await _BookController.DeleteBook(1) as NoContentResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task DeleteBook_InvalidId_ReturnsBadRequest()
        {
                   // Act
                var result = await _BookController.DeleteBook(0) as BadRequestObjectResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(400, result.StatusCode);
                Assert.AreEqual("Not a valid Book id", result.Value);
        }
    }
}
