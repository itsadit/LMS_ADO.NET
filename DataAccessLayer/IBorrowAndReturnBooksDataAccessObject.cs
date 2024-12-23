
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DataAccessLayer
{
    public interface IBorrowAndReturnBooksDataAccessObject
    {
        public IEnumerable<BorrowBooks> GetBorrowBooks();
        public IEnumerable<BorrowBooks> GetBorrowedBooksByUserID(int id);
        public IEnumerable<BorrowBooks> GetUsersWhoBorrowedBookbyBookName(string Book_name);
        public IEnumerable<BorrowBooks> GetUserWhoBorrowedBookbyBookID(int ID);

        public void BorrowBook(Request request );
        public void ReturnBook(Request request);
        public void RenewalBook(Request request);
        public void FinePayment(FinePayments fines);
        public IEnumerable<FinePayments> GetAllPayments();
        public IEnumerable<FinePayments> GetPayment(int id);
    }
}
