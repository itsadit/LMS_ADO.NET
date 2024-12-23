using System.Collections.Generic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.Enum;

namespace LibraryManagementSystem.BusinessLayer
{
    public interface IBookService
    {
        // Add a new book with its related details (bookName, author, genre, publisher)
        bool AddBook(string bookName, decimal bookPrice, string authors, string genres, string publishers);

        // Retrieve all books
        IEnumerable<Book> GetAllBooks();

        // Get a book with its related details (author, genre, publisher)
        Book GetBook(int bookId);

        // Update the details of a book
        bool UpdateBook(int bookId, string? bookName = null, decimal? bookPrice = null, string? authorName = null, string? genreName = null, string? publisherName = null);

        // Delete a book based on BookID
        bool DeleteBook(int bookId);

        // Search books based on a given field (e.g., name, author, genre)
        IEnumerable<Book> SearchBooks(SearchBy searchBy, string searchValue);
    }
}
