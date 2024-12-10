﻿using Library_Management_System.DataAccessLayer;
using Library_Management_System.Models;
using System.Xml.Linq;

namespace Library_Management_System.BLL
{
    public class BorrowAndReturnBookServices : IBorrowAndReturnBookServices
    {
        
        
        private readonly IBorrowAndReturnBooksDataAccessObject _dataAccessObject;

        public BorrowAndReturnBookServices(IBorrowAndReturnBooksDataAccessObject dataAccessObject)
        {
           _dataAccessObject = dataAccessObject;
        }
        

    // Implement methods that utilize _dataAccessObject

    public void BorrowBook(BorrowBooks books)
        {
            _dataAccessObject.BorrowBook(books);
        }

        public IEnumerable<BorrowBooks> GetAllUser_BookTransactions()
        {
            IEnumerable<BorrowBooks> BorrowBooks = _dataAccessObject.GetBorrowBooks();
            return BorrowBooks;
        }

        public IEnumerable<BorrowBooks> GetBorrowedBooksbyUserID(int ID)
        {
            IEnumerable<BorrowBooks> BorrowBooks = _dataAccessObject.GetBorrowedBooksByUserID(ID);
            return BorrowBooks;
        }

        public IEnumerable<BorrowBooks> GetUsersWhoBorrowedASpecificBookByName(string name)
        {
            IEnumerable<BorrowBooks> BorrowBooks = _dataAccessObject.GetUsersWhoBorrowedBookbyBookName(name);
            return BorrowBooks;
        }

        public IEnumerable<BorrowBooks> GetUserWhoBorrowedBookBYID(int ID)
        {
            IEnumerable<BorrowBooks> BorrowBooks = _dataAccessObject.GetUserWhoBorrowedBookbyBookID(ID);
            return BorrowBooks;
        }

        public void ReturnBook(BorrowBooks books)
        {
            _dataAccessObject.ReturnBook(books);
        }

        IEnumerable<FinePayments> IBorrowAndReturnBookServices.GetAllFinePayments()
        {
            IEnumerable<FinePayments> FinePayments = _dataAccessObject.GetAllPayments();
            return FinePayments;
        }

        IEnumerable<FinePayments> IBorrowAndReturnBookServices.GetFinePaymentsByID(int ID)
        {
            IEnumerable<FinePayments> FinePayments = _dataAccessObject.GetPayment(ID);
            return FinePayments;
        }

        void IBorrowAndReturnBookServices.PayFine(FinePayments Fines)
        {
            _dataAccessObject.FinePayment(Fines);
        }

        void IBorrowAndReturnBookServices.RenewalBook(BorrowBooks books)
        {
            _dataAccessObject.RenewalBook(books);
        }
    }
}