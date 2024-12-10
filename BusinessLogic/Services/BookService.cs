using LibraryAPI.BusinessLogic.Interfaces;
using LibraryAPI.DataAccess.Interfaces;
using LibraryAPI.DataAccess.Models;

namespace LibraryAPI.BusinessLogic.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // Add a new book with its related details (author, genre, publisher)
        public async Task<bool> AddBookAsync(string bookName, decimal bookPrice, string authors, string genres, string publishers)
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
                return await _bookRepository.AddBookAsync(book, authors, genres, publishers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in service layer: {ex.Message}");
                return false;
            }

        }

        // Retrieve all books
        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            try
            {
                return await Task.Run(() => _bookRepository.GetAllBooks());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving books: {ex.Message}");
                return new List<Book>();
            }
        }

        // Get a book with its related details (author, genre, publisher)
        public async Task<Book> GetBookAsync(int bookId)
        {
            try
            {
                return await Task.Run(() => _bookRepository.GetBookWithDetails(bookId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving book details: {ex.Message}");
                return null;
            }
        }

        // Update the details of a book
        public async Task<bool> UpdateBookAsync(int bookId, string? bookName = null, decimal? bookPrice = null, string? authorName = null, string? genreName = null, string? publisherName = null)
        {
            try
            {
                // Ensure a valid book ID is provided
                if (bookId <= 0)
                {
                    throw new ArgumentException("Invalid book ID.");
                }

                // Call the repository method with the nullable parameters
                await Task.Run(() =>
                    _bookRepository.UpdateBook(bookId, bookName, bookPrice, authorName, genreName, publisherName)
                );

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book: {ex.Message}");
                return false;
            }
        }

        // Delete a book based on BookID
        public async Task<bool> DeleteBookAsync(int bookId)
        {
            try
            {
                await Task.Run(() => _bookRepository.DeleteBook(bookId));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
                return false;
            }
        }

        // Search books based on a given field (e.g., name, author, genre)
        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchBy, string searchValue)
        {
            try
            {
                return await Task.Run(() => _bookRepository.SearchBooks(searchBy, searchValue));
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching books: {ex.Message}");
                return new List<Book>();
            }
        }
    }
}
