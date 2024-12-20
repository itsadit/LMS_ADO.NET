<<<<<<< HEAD
using CRUDS.DataAccess.Models.Enum;
=======
>>>>>>> 950b9cdb05c9e2c67eeafa9725168b4ba234d60a
using LibraryAPI.DataAccess.Models;

namespace LibraryAPI.BusinessLogic.Interfaces
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
<<<<<<< HEAD
        IEnumerable<Book> SearchBooks(SearchBy searchBy, string searchValue);
=======
        IEnumerable<Book> SearchBooks(string searchBy, string searchValue);
>>>>>>> 950b9cdb05c9e2c67eeafa9725168b4ba234d60a
    }
}
