using System.Collections.Generic;
using LibraryAPI.DataAccess.Models;

namespace LibraryAPI.DataAccess.Interfaces
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAllBooks();
        void UpdateBook(int bookId, string? bookName = null, decimal? bookPrice = null, string? authorName = null, string? genreName = null, string? publisherName = null);
        bool DeleteBook(int bookId);
        IEnumerable<Book> SearchBooks(string searchBy, string searchValue);
        Book GetBookWithDetails(int bookId);
        Task<bool> AddBookAsync(Book book, string authorName, string genreName, string publisherName);

    }

}
