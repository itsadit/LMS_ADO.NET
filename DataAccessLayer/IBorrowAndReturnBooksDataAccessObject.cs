using Library_Management_System.Models;

namespace Library_Management_System.DataAccessLayer
{
    public interface IBorrowAndReturnBooksDataAccessObject
    {
        public IEnumerable<BorrowBooks> GetBorrowBooks();
        public IEnumerable<BorrowBooks> GetBorrowedBooksByUserID(int id);
        public IEnumerable<BorrowBooks> GetUsersWhoBorrowedBookbyBookName(string Book_name);
        public IEnumerable<BorrowBooks> GetUserWhoBorrowedBookbyBookID(int ID);
        public void BorrowBook(BorrowBooks book);
        public void ReturnBook(BorrowBooks book);
        public void RenewalBook(BorrowBooks book);
        public void FinePayment(FinePayments fines);
        public IEnumerable<FinePayments> GetAllPayments();
        public IEnumerable<FinePayments> GetPayment(int id);
    }
}
