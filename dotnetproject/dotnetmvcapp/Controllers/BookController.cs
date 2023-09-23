using Microsoft.AspNetCore.Mvc;
using BookStoreApp.Models;
using BookStoreApp.Services;
using System;
using System.Threading.Tasks;

namespace BookStoreApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _BookService;

        public BookController(IBookService BookService)
        {
            _BookService = BookService;

        }

        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBook(Book book)
        {
            try
            {
                if (book == null)
                {
                    return BadRequest("Invalid Book data");
                }

                var success = _BookService.AddBook(book);

                if (success)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Failed to add the Book. Please try again.");
                return View(book);
            }
            catch (Exception ex)
            {
                // Log or print the exception to get more details
                Console.WriteLine("Exception: " + ex.Message);

                // Return an error response or another appropriate response
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again.");
                return View(book);
            }
        }

        public IActionResult Index()
        {
            try
            {
                var listBooks = _BookService.GetAllBooks();
                return View("Index",listBooks);
            }
            catch (Exception ex)
            {
                // Log or print the exception to get more details
                Console.WriteLine("Exception: " + ex.Message);

                // Return an error view or another appropriate response
                return View("Error"); // Assuming you have an "Error" view
            }
        }

        public IActionResult Search(int id)
        {
            try
            {
                var book = _BookService.GetBookById(id);

                if (book != null)
                {
                    return View("Search",new[] { book });
                }
                else
                {
                    return View("Search",new Book[0]);
                }
            }
            catch (Exception ex)
            {
                // Log or print the exception to get more details
                Console.WriteLine("Exception: " + ex.Message);

                // Return an error view or another appropriate response
                return View("Error"); // Assuming you have an "Error" view
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var success = _BookService.DeleteBook(id);

                if (success)
                {
                    // Check if the deletion was successful and return a view or a redirect
                    return RedirectToAction("Index"); // Redirect to the list of Books, for example
                }
                else
                {
                    // Handle other error cases
                    return View("Error"); // Assuming you have an "Error" view
                }
            }
            catch (Exception ex)
            {
                // Log or print the exception to get more details
                Console.WriteLine("Exception: " + ex.Message);

                // Return an error view or another appropriate response
                return View("Error"); // Assuming you have an "Error" view
            }
        }
    }
}
