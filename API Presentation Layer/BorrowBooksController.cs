using Library_Management_System.BLL;
using Library_Management_System.DataAccessLayer;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Library_Management_System.DataTransferObjects;
using static System.Reflection.Metadata.BlobBuilder;

namespace Library_Management_System.Controllers
{
    public class BorrowBooksController : Controller
    {
        private readonly IBorrowAndReturnBookServices _dataServicesObject;

        public BorrowBooksController(IBorrowAndReturnBookServices dataServicesObject)
        {
            _dataServicesObject = dataServicesObject;
        }
        [HttpPost("BorrowBook")]
        public IActionResult BorrowBook(Request request)
        {
            try
            {
                // Check if the bookId and userId are valid (you can add more validation here)
                if (request.BookId <= 0 || request.UserId <= 0)
                {
                    return BadRequest(new { message = "Invalid BookID or UserID" });
                }

                // Initialize the BorrowBooks object with necessary details
                var borrowBook = new BorrowBooks
                {
                    UserID = request.UserId,
                    BookID = request.BookId

                };

                // Call the BorrowBook method to insert the new record into the database
                _dataServicesObject.BorrowBook(borrowBook);

                return Ok(new { message = "Book borrowed successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (in real-world applications, log it to a logging system)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while borrowing the book", error = ex.Message });
            }
        }

        [HttpGet("Get All User-Book Transactions")]
        public IActionResult GetAllBorrowedBooks()
        {
            try
            {
                // Fetch all borrowed books status
                IEnumerable<BorrowBooks> borrowedBooks = _dataServicesObject.GetAllUser_BookTransactions();


                // Return the data as JSON
                return Json(borrowedBooks);
            }
            catch (Exception ex)
            {
                // Return an error message if something goes wrong
                return BadRequest(new { message = "An error occurred while retrieving borrowed books status", error = ex.Message });
            }
        }
        [HttpGet("GetBorrowBooksByUserId")]
        public IActionResult GetBorrowBooksByUserId(ByUserIDRequest request)
        {
            try
            {
                if (request.UserID <= 0)
                {
                    return BadRequest(new { message = "UserID Can't be Negative." });
                }
                // Call the method to fetch borrowed books for a specific user ID
                IEnumerable<BorrowBooks> borrowedBooks = _dataServicesObject.GetBorrowedBooksbyUserID(request.UserID);

                if (borrowedBooks == null || !borrowedBooks.Any())
                {
                    return NotFound(new { message = "No borrowed books found for this user." });
                }

                // Return the list of borrowed books as JSON
                return Json(borrowedBooks);
            }
            catch (Exception ex)
            {
                // Return an error message if something goes wrong
                return BadRequest(new { message = "An error occurred while retrieving borrowed books.", error = ex.Message });
            }
        }
        [HttpGet("GetUsersWhoBorrowedSpecificBook")]
        public IActionResult GetUsersWhoBorrowedBookbyBookName(ByBookNameRequest request)
        {
            try
            {
                IEnumerable<BorrowBooks> borrowedBooks = _dataServicesObject.GetUsersWhoBorrowedASpecificBookByName(request.BookName);

                if (borrowedBooks == null || !borrowedBooks.Any())
                {
                    return NotFound(new { message = "No users found who borrowed this book." });
                }

                return Json(borrowedBooks);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return BadRequest(new { message = "An error occurred while retrieving borrowed books status", error = ex.Message });
            }
        }

        [HttpGet("GetUsersWhoBorrowedBookByBookID/{ID}")]
        public IActionResult GetUserWhoBorrowedBookbyBookID(ByBookIDRequest request)
        {
            try
            {
                // Validate that the BookID is a valid number
                if (request.BookID <= 0)
                {
                    return BadRequest(new { message = "BookID Can't be Negative." });
                }

                // Call the data access method to fetch borrowed books by BookID
                IEnumerable<BorrowBooks> borrowedBooks = _dataServicesObject.GetUserWhoBorrowedBookBYID(request.BookID);

                // Check if no borrowed books found
                if (borrowedBooks == null || !borrowedBooks.Any())
                {
                    return NotFound(new { message = "No users found who borrowed this book." });
                }

                // Return the list of borrowed books as JSON
                return Json(borrowedBooks);
            }
            catch (Exception ex)
            {
                // Handle any errors and return a BadRequest response
                return BadRequest(new { message = "An error occurred while retrieving borrowed books.", error = ex.Message });
            }
        }
    }
}
