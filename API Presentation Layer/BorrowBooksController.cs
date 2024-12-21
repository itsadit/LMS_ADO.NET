using Library_Management_System.BLL;
using Library_Management_System.DataAccessLayer;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
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
        /// <summary>
        /// Takes BookID and UserID as input and Assigns Book to the User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("BorrowBook")]
        public IActionResult BorrowBook([FromForm] Request request)
        {
            if (!ModelState.IsValid)
            {
                // Return validation errors
                return BadRequest(ModelState);
            }

            try
            {
                // Initialize the BorrowBooks object with necessary details
                var borrowBook = new BorrowBooks
                {
                    UserID = request.UserID,
                    BookID = request.BookID
                };

                // Call the BorrowBook method to insert the new record into the database
                _dataServicesObject.BorrowBook(request);

                return Ok(new { message = "Book borrowed successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (in real-world applications, log it to a logging system)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while borrowing the book", error = ex.Message });
            }
        }

        /// <summary>
        /// On Execution Returns All the Details of Books Borrowed By Different Users
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUserBookTransactions")]
        public IActionResult GetAllBorrowedBooks()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Return validation errors
                    return BadRequest(ModelState);
                }
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
        /// <summary>
        /// Takes UserID as Input and Returns All the Books Borrowed Books By that User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("GetBorrowBooksByUserId")]
        public IActionResult GetBorrowBooksByUserId([FromQuery] ByUserIDRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Return validation errors
                    return BadRequest(ModelState);
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
        /// <summary>
        /// Takes BookName As Input and Returns all the Users Who Borrowed Books With that BookName
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("GetUsersWhoBorrowedBookByBookName")]
        public IActionResult GetUsersWhoBorrowedBookbyBookName([FromQuery] ByBookNameRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Return validation errors
                    return BadRequest(ModelState);
                }
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
        /// <summary>
        /// Takes BookID as Input and Returns all the Books Details Who Borrowed that Specific Book
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("GetUsersWhoBorrowedSpecificBookByBookID")]
        public IActionResult GetUserWhoBorrowedBookbyBookID([FromQuery] ByBookIDRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Return validation errors
                    return BadRequest(ModelState);
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
