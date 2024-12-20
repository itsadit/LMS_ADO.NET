using System.Collections.Generic;
using CRUDS.DataAccess.Models.Enum;
using LibraryAPI.DataAccess.Models;

namespace LibraryAPI.DataAccess.Interfaces
{
    /// <summary>
    /// Interface for book repository defining CRUD operations and additional functionalities for managing books.
    /// </summary>
    public interface IBookRepository
    {
        /// <summary>
        /// Retrieves all books from the repository.
        /// </summary>
        /// <returns>A collection of all books.</returns>
        IEnumerable<Book> GetAllBooks();

        /// <summary>
        /// Updates an existing book's details in the repository. 
        /// Only the provided parameters will be updated.
        /// </summary>
        /// <param name="bookId">The ID of the book to update.</param>
        /// <param name="bookName">The new name of the book (optional).</param>
        /// <param name="bookPrice">The new price of the book (optional).</param>
        /// <param name="authorName">The new author name for the book (optional).</param>
        /// <param name="genreName">The new genre name for the book (optional).</param>
        /// <param name="publisherName">The new publisher name for the book (optional).</param>
        void UpdateBook(
            int bookId,
            string? bookName = null,
            decimal? bookPrice = null,
            string? authorName = null,
            string? genreName = null,
            string? publisherName = null
        );

        /// <summary>
        /// Deletes a book from the repository by its ID.
        /// </summary>
        /// <param name="bookId">The ID of the book to delete.</param>
        /// <returns>True if the book was successfully deleted; otherwise, false.</returns>
        bool DeleteBook(int bookId);

        /// <summary>
        /// Searches for books in the repository based on a specific field and value.
        /// </summary>
        /// <param name="searchBy">The field to search by (e.g., "Author", "Genre").</param>
        /// <param name="searchValue">The value to search for in the specified field.</param>
        /// <returns>A collection of books that match the search criteria.</returns>
        IEnumerable<Book> SearchBooks(SearchBy searchBy, string searchValue);
        /// <summary>
        /// Retrieves a specific book from the repository by its ID.
        /// </summary>
        /// <param name="bookId">The ID of the book to retrieve.</param>
        /// <returns>The book with the specified ID.</returns>
        Book GetBookWithId(int bookId);

        /// <summary>
        /// Adds a new book to the repository along with its associated author, genre, and publisher details.
        /// </summary>
        /// <param name="book">The book object containing basic book details.</param>
        /// <param name="authorName">The name of the author associated with the book.</param>
        /// <param name="genreName">The genre of the book.</param>
        /// <param name="publisherName">The publisher of the book.</param>
        /// <returns>True if the book was successfully added; otherwise, false.</returns>
        bool AddBook(Book book, string authorName, string genreName, string publisherName);
        
    }
}
