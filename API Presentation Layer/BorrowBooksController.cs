﻿using Library_Management_System.BLL;
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
        [HttpPost("BorrowBook")]
        public IActionResult BorrowBook(int bookId, int userId)
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

        [HttpGet("GetAllUserBookTransactions")]
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
        public IActionResult GetBorrowBooksByUserId([FromQuery, Required] int id)
        {
            try
            {
                // Call the method to fetch borrowed books for a specific user ID
                IEnumerable<BorrowBooks> borrowedBooks = _dataServicesObject.GetBorrowedBooksbyUserID(id);

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
        public IActionResult GetUsersWhoBorrowedBookbyBookName([FromQuery, Required] string Book_Name)
        {
            try
            {
                IEnumerable<BorrowBooks> borrowedBooks = _dataServicesObject.GetUsersWhoBorrowedASpecificBookByName(Book_Name);

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

        [HttpGet("GetUsersWhoBorrowedBookByBookID")]
        public IActionResult GetUserWhoBorrowedBookbyBookID(int ID)
        {
            try
            {
                // Validate that the BookID is a valid number
                if (ID <= 0)
                {
                    return BadRequest(new { message = "Invalid BookID." });
                }

                // Call the data access method to fetch borrowed books by BookID
                IEnumerable<BorrowBooks> borrowedBooks = _dataServicesObject.GetUserWhoBorrowedBookBYID(ID);

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