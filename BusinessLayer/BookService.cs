using LibraryManagementSystem.DataAccessLayer;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.Enum;

namespace LibraryManagementSystem.BusinessLayer
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        /// <summary>
        /// Adds a new book to the database with related authors, genres, and publishers.
        /// </summary>
        /// <param name="bookName">The name of the book.</param>
        /// <param name="bookPrice">The price of the book.</param>
        /// <param name="authors">The list of authors related to the book.</param>
        /// <param name="genres">The list of genres related to the book.</param>
        /// <param name="publishers">The list of publishers related to the book.</param>
        /// <returns>True if the book was successfully added; otherwise, false.</returns>
        public bool AddBook(string bookName, decimal bookPrice, string authors, string genres, string publishers)
        {
            // Create a new Book object using the received parameters
            var book = new Book
            {
                BookName = bookName,
                BookPrice = bookPrice
            };

            // Call the repository to add the book to the database
            try
            {
                return _bookRepository.AddBook(book, authors, genres, publishers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in service layer: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves all books from the database.
        /// </summary>
        /// <returns>A list of all books.</returns>
        public IEnumerable<Book> GetAllBooks()
        {
            try
            {
                return _bookRepository.GetAllBooks();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving books: {ex.Message}");
                return new List<Book>();
            }
        }

        /// <summary>
        /// Retrieves a book with its related details (author, genre, publisher) by book ID.
        /// </summary>
        /// <param name="bookId">The ID of the book to retrieve.</param>
        /// <returns>The book with its related details, or null if not found.</returns>
        public Book GetBook(int bookId)
        {
            try
            {
                return _bookRepository.GetBookWithId(bookId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving book details: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Updates the details of a book.
        /// </summary>
        /// <param name="bookId">The ID of the book to update.</param>
        /// <param name="bookName">The updated book name.</param>
        /// <param name="bookPrice">The updated book price.</param>
        /// <param name="authorName">The updated author name.</param>
        /// <param name="genreName">The updated genre name.</param>
        /// <param name="publisherName">The updated publisher name.</param>
        /// <returns>True if the book was successfully updated; otherwise, false.</returns>

        public bool UpdateBook(int bookId, string? bookName = null, decimal? bookPrice = null, string? authorName = null, string? genreName = null, string? publisherName = null)
        {
            try
            {
                // Ensure a valid book ID is provided
                if (bookId <= 0)
                {
                    throw new ArgumentException("Invalid book ID.");
                }

                // Call the repository method with the nullable parameters
                _bookRepository.UpdateBook(bookId, bookName, bookPrice, authorName, genreName, publisherName);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a book based on its ID.
        /// </summary>
        /// <param name="bookId">The ID of the book to delete.</param>
        /// <returns>True if the book was successfully deleted; otherwise, false.</returns>

        public bool DeleteBook(int bookId)
        {
            try
            {
                _bookRepository.DeleteBook(bookId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Searches for books based on a given field (e.g., name, author, genre).
        /// </summary>
        /// <param name="searchBy">The field to search by (e.g., name, author, genre).</param>
        /// <param name="searchValue">The value to search for.</param>
        /// <returns>A list of books that match the search criteria.</returns>


        public IEnumerable<Book> SearchBooks(SearchBy searchBy, string searchValue)

        {
            try
            {
                return _bookRepository.SearchBooks(searchBy, searchValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching books: {ex.Message}");
                return new List<Book>();
            }
        }
    }
}
