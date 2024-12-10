using Library_Management_System.Models;

namespace Library_Management_System.DataAccessLayer
{
    public interface IBooksDataAccessObjects
    {
        public IEnumerable<Books> GetBooks();
    }
}
