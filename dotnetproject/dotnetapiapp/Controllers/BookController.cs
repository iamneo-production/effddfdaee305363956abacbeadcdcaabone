using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotnetapiapp.Models;

namespace dotnetapiapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookStoreContext _context;

        public BookController(BookStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            var books = await _context.Books.ToListAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // [HttpGet("total")]
        // public async Task<ActionResult<int>> GetTotalNumberOfBooks()
        // {
        //     var totalBooks = await _context.Books.CountAsync();
        //     return Ok(totalBooks);
        // }
        [HttpPost]
        public async Task<ActionResult> AddBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return detailed validation errors
            }
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid Book id");

            var book = await _context.Books.FindAsync(id);
              _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
