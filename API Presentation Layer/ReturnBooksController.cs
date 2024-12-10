using Library_Management_System.BLL;
using Library_Management_System.DataAccessLayer;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace Library_Management_System.APIs___Controllers
{
    
    public class ReturnBooksController : ControllerBase
    {
        
        private readonly IBorrowAndReturnBookServices _dataServicesObject;

        // Corrected constructor to inject BorrowAndReturnBookServices instead of IConfiguration
        public ReturnBooksController(IBorrowAndReturnBookServices dataServicesObject)
        {
            _dataServicesObject = dataServicesObject;
        }
        [HttpPut("ReturnBook")]
        public IActionResult RetrieveBook( [Required] int bookId, [Required] int userId)
        {
            try
            {
                // Check if the bookId and userId are valid (you can add more validation here)
                if (bookId <= 0 || userId <= 0)
                {
                    return BadRequest(new { message = "Invalid BookID or UserID" });
                }

                // Initialize the BorrowBooks object with necessary details
                var borrowBook = new BorrowBooks
                {
                    UserID = userId,
                    BookID = bookId

                };

                // Call the BorrowBook method to insert the new record into the database
                _dataServicesObject.ReturnBook(borrowBook);

                return Ok(new { message = "Book Returned successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (in real-world applications, log it to a logging system)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while Returned the book", error = ex.Message });
            }
        }
        [HttpPut("RenewalBook")]
        public IActionResult RenewalBook([Required] int bookId,[Required] int userId)
        {
            try
            {
                // Check if the bookId and userId are valid (you can add more validation here)
                if (bookId <= 0 || userId <= 0)
                {
                    return BadRequest(new { message = "Invalid BookID or UserID" });
                }

                // Initialize the BorrowBooks object with necessary details
                var borrowBook = new BorrowBooks
                {
                    UserID = userId,
                    BookID = bookId
                };

                // Call the BorrowBook method to insert the new record into the database
                _dataServicesObject.RenewalBook(borrowBook);

                return Ok(new { message = "Book Renewed successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (in real-world applications, log it to a logging system)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while Renewing the book", error = ex.Message });
            }
        }
        [HttpPost("PayFine")]
        public IActionResult FinePayment([Required] int userId, [Required] decimal Amount, [Required] string PaymentMethod, [Required] string TransactionID)
        {
            try
            {
                // Check if the bookId and userId are valid (you can add more validation here)
                if ( Amount <=0|| userId <= 0)
                {
                    return BadRequest(new { message = "Invalid UserID or Amount" });
                }

                // Initialize the BorrowBooks object with necessary details
                var FinePayments = new FinePayments
                {
                    UserID = userId,
                    Amount = Amount,
                    PaymentMethod = PaymentMethod,
                    TransactionID = TransactionID

                };

                // Call the BorrowBook method to insert the new record into the database
                _dataServicesObject.PayFine(FinePayments);

                return Ok(new { message = "Fine Paid successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (in real-world applications, log it to a logging system)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while Paying the Fine", error = ex.Message });
            }
        }
        [HttpGet("GetAllPaymentDetails")]
        public IActionResult GetAllPayments()
        {
            try
            {
                // Fetch all borrowed books status
                IEnumerable<FinePayments> Fines = _dataServicesObject.GetAllFinePayments();


                // Return the data as JSON
                return Ok(Fines);
            }
            catch (Exception ex)
            {
                // Return an error message if something goes wrong
                return BadRequest(new { message = "An error occurred while retrieving Payment Details", error = ex.Message });
            }
        }
        [HttpGet("GetPaymentDetailsByUserID")]
        public IActionResult GetPaymentsByUserID([Required] int ID)
        {
            try
            {
                // Fetch all borrowed books status
                IEnumerable<FinePayments> Fines = _dataServicesObject.GetFinePaymentsByID(ID);


                // Return the data as JSON
                return Ok(Fines);
            }
            catch (Exception ex)
            {
                // Return an error message if something goes wrong
                return BadRequest(new { message = "An error occurred while retrieving Payment Details", error = ex.Message });
            }
        }
    }
}
