using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryAPI.DataAccess.Models;

namespace LibraryAPI.BusinessLogic.Interfaces
{
    public interface IBookService
    {
        // Add a new book with its related details (bookName, author, genre, publisher)
        Task<bool> AddBookAsync(string bookName, decimal bookPrice, string authors, string genres, string publishers);

        // Retrieve all books
        Task<IEnumerable<Book>> GetAllBooksAsync();

        // Get a book with its related details (author, genre, publisher)
        Task<Book> GetBookAsync(int bookId);

        // Update the details of a book
        Task<bool> UpdateBookAsync(int bookId, string? bookName = null, decimal? bookPrice = null, string? authorName = null, string? genreName = null, string? publisherName = null);

        // Delete a book based on BookID
        Task<bool> DeleteBookAsync(int bookId);

        // Search books based on a given field (e.g., name, author, genre)
        Task<IEnumerable<Book>> SearchBooksAsync(string searchBy, string searchValue);
    }
}
