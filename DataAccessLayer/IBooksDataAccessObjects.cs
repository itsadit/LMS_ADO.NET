using LibraryManagementSystem.Models;

namespace Library_Management_System.DataAccessLayer
{
    public interface IBooksDataAccessObjects
    {
        public IEnumerable<Book> GetBooks();
    }
}
