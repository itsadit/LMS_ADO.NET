using Library_Management_System.Models;

namespace Library_Management_System.BLL
{
    public interface IBorrowAndReturnBookServices
    {
        public IEnumerable<BorrowBooks> GetAllUser_BookTransactions();
        public IEnumerable<BorrowBooks> GetBorrowedBooksbyUserID(int ID);
        public IEnumerable<BorrowBooks> GetUsersWhoBorrowedASpecificBookByName(string name);
        public IEnumerable<BorrowBooks> GetUserWhoBorrowedBookBYID(int ID);
        public void BorrowBook(BorrowBooks books);
        public void ReturnBook(BorrowBooks books);
        public void RenewalBook(BorrowBooks books);
        public void PayFine(FinePayments Fines);
        public IEnumerable<FinePayments> GetAllFinePayments();
        public IEnumerable<FinePayments> GetFinePaymentsByID(int ID);

    }
}
