using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.BusinessLayer
{
    public interface IBorrowAndReturnBookServices
    {
        public IEnumerable<BorrowBooks> GetAllUser_BookTransactions();
        public IEnumerable<BorrowBooks> GetBorrowedBooksbyUserID(int ID);
        public IEnumerable<BorrowBooks> GetUsersWhoBorrowedASpecificBookByName(string name);
        public IEnumerable<BorrowBooks> GetUserWhoBorrowedBookBYID(int ID);
        public void BorrowBook(Request request);
        public void ReturnBook(Request request);
        public void RenewalBook(Request request);
        public void PayFine(FinePayments Fines);
        public IEnumerable<FinePayments> GetAllFinePayments();
        public IEnumerable<FinePayments> GetFinePaymentsByID(int ID);

    }
}
