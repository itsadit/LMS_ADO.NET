using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models.Enum;
using Swashbuckle.AspNetCore.Annotations;
using LibraryManagementSystem.Models.DTO;
using LibraryManagementSystem.BusinessLayer;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Presentation.Controllers
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

        /// <summary>
        /// Add a new book with its related details (e.g., author, genre, publisher)
        /// </summary>
        /// <param name="request">The book details to be added.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("books")]
        [SwaggerOperation(Summary = "Add a new book along with its author, genre, and publisher",
        Description = "Use this endpoint to add a new book with associated details like authors, genres, and publishers.")]
        public IActionResult AddBook([FromForm] AddBookRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Console.WriteLine($"Received BookName: {request.BookName}, BookPrice: {request.BookPrice}, Authors: {request.Authors}, Genres: {request.Genres}, Publishers: {request.Publishers}");

            var result = _bookService.AddBook(request.BookName, request.BookPrice, request.Authors, request.Genres, request.Publishers);

            if (result)
            {
                return Ok(new { message = "Book added successfully" });
            }

            return BadRequest("Failed to add book. Please check your inputs.");
        }

        /// <summary>
        /// Retrieve a list of all books
        /// </summary>
        /// <returns>A list of all books in the system.</returns>
        [Authorize(Roles = "Admin, User")]
        [HttpGet("books")]
        [SwaggerOperation(Summary = "Retrieve a list of all books",
        Description = "Use this endpoint to get a list of all books in the system.")]
        public IActionResult GetAllBooks()
        {
            var books = _bookService.GetAllBooks();
            if (books == null || !books.Any())
            {
                return NotFound("No books found.");
            }
            return Ok(books);
        }

        /// <summary>
        /// Retrieve a specific book by its ID
        /// </summary>
        /// <param name="id">The unique ID of the book to retrieve.</param>
        /// <returns>The details of the book if found, otherwise an error message.</returns>
        [Authorize(Roles = "Admin, User")]
        [HttpGet("books/{id}")]
        [SwaggerOperation(Summary = "Retrieve a specific book by its ID", Description = "Use this endpoint to retrieve a specific book by providing its unique ID.")]
        public IActionResult GetBook([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid book ID.");
            }

            var book = _bookService.GetBook(id);
            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }
            return Ok(book);
        }

        /// <summary>
        /// Update the details of a book
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="request">The updated book details.</param>
        /// <returns>A response indicating the result of the update operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("books/{id}")]
        [SwaggerOperation(Summary = "Update the details of a book",
        Description = "Use this endpoint to update the details (name, price, author, genre, publisher) of an existing book.")]
        public IActionResult UpdateBook(int id, [FromForm] UpdateBookRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid book ID.");
            }

            try
            {
                var result = _bookService.UpdateBook(id, request.BookName, request.BookPrice, request.AuthorName, request.GenreName, request.PublisherName);

                if (result)
                {
                    return Ok(new { message = "Book updated successfully" });
                }

                return BadRequest($"Book ID {id} is invalid.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a specific book by its ID
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>A response indicating whether the deletion was successful.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("books/{id}")]
        [SwaggerOperation(Summary = "Delete a specific book by its ID",
        Description = "Use this endpoint to delete a specific book by providing its unique ID.")]
        public IActionResult DeleteBook([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid book ID.");
            }

            var result = _bookService.DeleteBook(id);
            if (result)
            {
                return Ok(new { message = "Book deleted successfully" });
            }

            return NotFound("No books found matching the search criteria.");
        }

        /// <summary>
        /// Search for books based on a given field (e.g., name, author, genre)
        /// </summary>
        /// <param name="request">The search criteria (search field and value).</param>
        /// <returns>A list of books matching the search criteria.</returns>
        [Authorize(Roles = "Admin, User")]
        [HttpGet("books/search")]
        [SwaggerOperation(Summary = "Search for books based on a given field",
        Description = "Use this endpoint to search for books by specifying a search field (e.g., name, author, genre) and its corresponding value.")]
        public IActionResult SearchBooks([FromQuery] SearchBooksRequest request)
        {

            // Check if the search value is provided
            if (string.IsNullOrEmpty(request.SearchValue))
            {
                return BadRequest("Search value is missing.");
            }

            // Try to parse the SearchBy value to the enum
            if (!Enum.TryParse<SearchBy>(request.SearchBy.ToString(), true, out var searchBy))
            {
                return BadRequest("Invalid search criteria.");
            }

            // Call the service method with the enum-based search criteria
            var books = _bookService.SearchBooks(searchBy, request.SearchValue);

            if (books == null || !books.Any())
            {
                return NotFound("No books found matching the search criteria.");
            }

            return Ok(books);
        }
    }
}
