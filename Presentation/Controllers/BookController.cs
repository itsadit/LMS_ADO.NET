using LibraryAPI.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LibraryAPI.DataAccess.Models;

namespace LibraryAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // Add a new book with its related details (author, genre, publisher)
        [HttpPost("add-book")]
        public async Task<IActionResult> AddBookAsync(
            [FromQuery] string bookName,
            [FromQuery] decimal bookPrice,
            [FromQuery] string authors,
            [FromQuery] string genres,
            [FromQuery] string publishers)
        {
            Console.WriteLine($"Received BookName: {bookName}, BookPrice: {bookPrice}, Authors: {authors}, Genres: {genres}, Publishers: {publishers}");

            // Validate the input types according to schema
            if (bookPrice <= 0)
            {
                return BadRequest("BookPrice must be a positive value.");
            }
            // Check if the required fields are not null or empty
            if (string.IsNullOrEmpty(bookName) ||
                string.IsNullOrEmpty(authors) ||
                string.IsNullOrEmpty(genres) ||
                string.IsNullOrEmpty(publishers))
            {
                return BadRequest("All fields (BookName, Authors, Genres, and Publishers) are required and cannot be empty.");
            }

            // Call the AddBookAsync method from the service layer
            var result = await _bookService.AddBookAsync(bookName, bookPrice, authors, genres, publishers);

            if (result)
            {
                return Ok(new { message = "Book added successfully" });
            }

            return BadRequest("Failed to add book. Please check your inputs.");
        }

        // Retrieve all books
        [HttpGet("all-books")]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            var books = await _bookService.GetAllBooksAsync();
            if (books == null || !books.Any())
            {
                return NotFound("No books found.");
            }
            return Ok(books);
        }

        // Get a book with its related details (author, genre, publisher)
        [HttpGet("search-with-id/{id}")]
        public async Task<IActionResult> GetBookAsync(int id)
        {
            var book = await _bookService.GetBookAsync(id);
            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }
            return Ok(book);
        }

        // Update the details of a book
        [HttpPut("update-by-id/{id}")]
        public async Task<IActionResult> UpdateBookAsync(
        int id,
        [FromQuery] string? bookName = null,
        [FromQuery] decimal? bookPrice = null,
        [FromQuery] string? authorName = null,
        [FromQuery] string? genreName = null,
        [FromQuery] string? publisherName = null)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid book ID.");
            }

            try
            {
                var result = await _bookService.UpdateBookAsync(
                    id,
                    bookName,
                    bookPrice,
                    authorName,
                    genreName,
                    publisherName
                );

                if (result)
                {
                    return Ok(new { message = "Book updated successfully" });
                }

                return BadRequest("Failed to update the book.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }




        // Delete a book based on BookID
        [HttpDelete("delete-by-id/{id}")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            // Call the DeleteBook method in the service layer
            var result = await _bookService.DeleteBookAsync(id);

            if (result)
            {
                // If book is deleted successfully, return Ok
                return Ok(new { message = "Book deleted successfully" });
            }

            // If the book is not found, return NotFound with a message
            return NotFound("No books found matching the search criteria.");
        }




        // Search books based on a given field (e.g., name, author, genre)
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooksAsync([FromQuery] string searchBy, [FromQuery] string searchValue)
        {
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchValue))
            {
                return BadRequest("Search criteria is missing.");
            }
            Console.WriteLine($"SearchBy: {searchBy}, SearchValue: {searchValue}");     
            var books = await _bookService.SearchBooksAsync(searchBy, searchValue);
            if (books == null || !books.Any())
            {
                return NotFound("No books found matching the search criteria.");
            }

            return Ok(books);
        }

    }
}
