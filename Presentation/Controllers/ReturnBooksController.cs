using LibraryManagementSystem.BusinessLayer;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Presentation.Controllers
{
    public class ReturnBooksController : ControllerBase
    {

        private readonly IBorrowAndReturnBookServices _dataServicesObject;

        // Corrected constructor to inject BorrowAndReturnBookServices instead of IConfiguration
        public ReturnBooksController(IBorrowAndReturnBookServices dataServicesObject)
        {
            _dataServicesObject = dataServicesObject;
        }
        /// <summary>
        /// Takes UserID and BookID as Input and Updates the Return Date and Fines in the DataBase
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, User")]
        [HttpPut("ReturnBook")]
        public IActionResult RetrieveBook([FromForm] Request request)
        {
            try
            {
                // Validate the request object
                if (!ModelState.IsValid)
                {
                    // Return validation errors
                    return BadRequest(ModelState);
                }

                // Initialize the BorrowBooks object with necessary details
                var borrowBook = new BorrowBooks
                {
                    UserID = request.UserID,
                    BookID = request.BookID
                };
                // Call the ReturnBook method to update the record in the database
                _dataServicesObject.ReturnBook(request);

                return Ok(new { message = "Book Returned successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (in real-world applications, log it to a logging system)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while returning the book", error = ex.Message });
            }
        }
        /// <summary>
        /// Takes UserID and BookID as Input and Increments the Due date
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("RenewalBook")]
        public IActionResult RenewalBook([FromForm] Request request)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    // Return validation errors
                    return BadRequest(ModelState);
                }

                // Initialize the BorrowBooks object with necessary details
                var borrowBook = new BorrowBooks
                {
                    UserID = request.BookID,
                    BookID = request.UserID
                };

                // Call the BorrowBook method to insert the new record into the database
                _dataServicesObject.RenewalBook(request);

                return Ok(new { message = "Book Renewed successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (in real-world applications, log it to a logging system)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while Renewing the book", error = ex.Message });
            }
        }
        /// <summary>
        /// Takes UserID and Amount as Input and Updates Total Fine in Users Table and Fine Paid in Borrowed Books Table
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("PayFine")]
        public IActionResult FinePayment([FromForm] FinePaymentRequest request)
        {
            try
            {
                // Check if the request is valid (amount and userId should be greater than zero)
                if (!ModelState.IsValid)
                {
                    // Return validation errors
                    return BadRequest(ModelState);
                }

                // Initialize the FinePayments object with the details from the request
                var finePayment = new FinePayments
                {
                    UserID = request.UserID,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod.ToString(),
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
        /// <summary>
        /// Returns All the Payments Details
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
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
        /// <summary>
        /// Takes UserID as Input and Returns All the Payments Done By that User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetPaymentDetailsByUserID")]
        public IActionResult GetPaymentsByUserID([FromForm] ByUserIDRequest request)
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
