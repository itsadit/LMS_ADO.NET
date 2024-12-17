using Library_Management_System.BLL;
using Library_Management_System.DataAccessLayer;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using Library_Management_System.DataTransferObjects;

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
        public IActionResult RetrieveBook(Request request)
        {
            try
            {
                // Validate the request object
                if (request == null || request.BookId <= 0 || request.UserId <= 0)
                {
                    return BadRequest(new { message = "BookID or UserID Can't be Negative" });
                }

                // Initialize the BorrowBooks object with necessary details
                var borrowBook = new BorrowBooks
                {
                    UserID = request.UserId,
                    BookID = request.BookId
                };

                // Call the ReturnBook method to update the record in the database
                _dataServicesObject.ReturnBook(borrowBook);

                return Ok(new { message = "Book Returned successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (in real-world applications, log it to a logging system)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while returning the book", error = ex.Message });
            }
        }

        [HttpPut("RenewalBook")]
        public IActionResult RenewalBook(Request request)
        {
            try
            {
                // Check if the bookId and userId are valid (you can add more validation here)
                if (request.BookId <= 0 || request.UserId <= 0)
                {
                    return BadRequest(new { message = "BookID or UserID Can't be Negative" });
                }

                // Initialize the BorrowBooks object with necessary details
                var borrowBook = new BorrowBooks
                {
                    UserID = request.BookId,
                    BookID = request.UserId
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
        public IActionResult FinePayment(FinePaymentRequest request)
        {
            try
            {
                // Check if the request is valid (amount and userId should be greater than zero)
                if (request.Amount <= 0 || request.UserId <= 0)
                {
                    return BadRequest(new { message = "UserID or Amount Can't be Negative" });
                }

                // Initialize the FinePayments object with the details from the request
                var finePayment = new FinePayments
                {
                    UserID = request.UserId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    TransactionID = request.TransactionID
                };

                // Call the PayFine method to process the payment
                _dataServicesObject.PayFine(finePayment);

                return Ok(new { message = "Fine Paid successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (in real-world applications, log it to a logging system)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while paying the fine", error = ex.Message });
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
        public IActionResult GetPaymentsByUserID(ByUserIDRequest request)
        {
            try
            {
                // Fetch the fine payment details by user ID
                IEnumerable<FinePayments> finePayments = _dataServicesObject.GetFinePaymentsByID(request.UserID);

                // If no data is found, return a not found message
                if (finePayments == null || !finePayments.Any())
                {
                    return NotFound(new { message = "No payment details found for the provided UserID." });
                }

                // Return the fine payment details as a JSON response
                return Ok(finePayments);
            }
            catch (Exception ex)
            {
                // Log the error (consider using a logging framework for production)
                Console.WriteLine($"An error occurred while retrieving payment details for user {request.UserID}: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving payment details", error = ex.Message });
            }
        }

    }
}
