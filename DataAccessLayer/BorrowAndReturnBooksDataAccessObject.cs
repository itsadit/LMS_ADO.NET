using Library_Management_System.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Data.SqlClient;

namespace Library_Management_System.DataAccessLayer
{
    public class BorrowAndReturnBooksDataAccessObject : IBorrowAndReturnBooksDataAccessObject
    {
        public IConfiguration configuration { get; }
        public BorrowAndReturnBooksDataAccessObject(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<BorrowBooks> GetBorrowBooks()
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];
            List<BorrowBooks> Borrowedbooks = new List<BorrowBooks>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"SELECT BorrowedBooks.*,Books.BookName FROM BorrowedBooks INNER JOIN Books on Books.BookID = BorrowedBooks.BookID";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();

                // Fill the data table with the result of the query
                dataAdapter.Fill(dataTable);

                // Convert the data rows into BorrowBooks objects
                foreach (DataRow datarow in dataTable.Rows)
                {
                    BorrowBooks borrowBook = new BorrowBooks
                    {
                        BorrowID = Convert.ToInt32(datarow["BorrowID"]),
                        UserID = Convert.ToInt32(datarow["UserID"]),
                        BookID = Convert.ToInt32(datarow["BookID"]),
                        BookName = datarow["BookName"]?.ToString(),
                        BorrowDate = Convert.ToDateTime(datarow["BorrowDate"]),
                        DueDate = Convert.ToDateTime(datarow["DueDate"]),
                        ReturnDate = datarow["ReturnDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(datarow["ReturnDate"]),
                        RenewalCount = Convert.ToInt32(datarow["RenewalCount"]),
                        FineAmount = datarow["FineAmount"] == DBNull.Value ? 0 : Convert.ToInt32(datarow["FineAmount"]),
                        IsFinePaid = datarow["IsFinePaid"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(datarow["IsFinePaid"])
                    };
                    Borrowedbooks.Add(borrowBook);
                }
            }

            return Borrowedbooks;
        }
        public IEnumerable<BorrowBooks> GetBorrowedBooksByUserID(int id)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];
            List<BorrowBooks> Borrowedbooks = new List<BorrowBooks>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"SELECT BorrowedBooks.*,Books.BookName FROM BorrowedBooks INNER JOIN Books on Books.BookID = BorrowedBooks.BookID WHERE UserID = {id}";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();

                // Fill the data table with the result of the query
                dataAdapter.Fill(dataTable);

                // Convert the data rows into BorrowBooks objects
                foreach (DataRow datarow in dataTable.Rows)
                {
                    BorrowBooks borrowBook = new BorrowBooks
                    {
                        BorrowID = Convert.ToInt32(datarow["BorrowID"]),
                        UserID = Convert.ToInt32(datarow["UserID"]),
                        BookID = Convert.ToInt32(datarow["BookID"]),
                        BookName = datarow["BookName"]?.ToString(),
                        BorrowDate = Convert.ToDateTime(datarow["BorrowDate"]),
                        DueDate = Convert.ToDateTime(datarow["DueDate"]),
                        ReturnDate = datarow["ReturnDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(datarow["ReturnDate"]),
                        RenewalCount = Convert.ToInt32(datarow["RenewalCount"]),
                        FineAmount = datarow["FineAmount"] == DBNull.Value ? 0 : Convert.ToInt32(datarow["FineAmount"]),
                        IsFinePaid = datarow["IsFinePaid"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(datarow["IsFinePaid"])
                    };
                    Borrowedbooks.Add(borrowBook);
                }
            }

            return Borrowedbooks;
        }
        public IEnumerable<BorrowBooks> GetUserWhoBorrowedBookbyBookID(int ID)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];
            List<BorrowBooks> Borrowedbooks = new List<BorrowBooks>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"Select BorrowedBooks.*,Books.BookName From BorrowedBooks " +
                    $"INNER JOIN Books ON Books.BookID = BorrowedBooks.BookID " +
                    $"where Books.BookID ={ID}";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();

                // Fill the data table with the result of the query
                dataAdapter.Fill(dataTable);

                // Convert the data rows into BorrowBooks objects
                foreach (DataRow datarow in dataTable.Rows)
                {
                    BorrowBooks borrowBook = new BorrowBooks
                    {
                        BorrowID = Convert.ToInt32(datarow["BorrowID"]),
                        UserID = Convert.ToInt32(datarow["UserID"]),
                        BookID = Convert.ToInt32(datarow["BookID"]),
                        BookName = datarow["BookName"]?.ToString(),
                        BorrowDate = Convert.ToDateTime(datarow["BorrowDate"]),
                        DueDate = Convert.ToDateTime(datarow["DueDate"]),
                        ReturnDate = datarow["ReturnDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(datarow["ReturnDate"]),
                        RenewalCount = Convert.ToInt32(datarow["RenewalCount"]),
                        FineAmount = datarow["FineAmount"] == DBNull.Value ? 0 : Convert.ToInt32(datarow["FineAmount"]),
                        IsFinePaid = datarow["IsFinePaid"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(datarow["IsFinePaid"])
                    };
                    Borrowedbooks.Add(borrowBook);
                }
            }
            return Borrowedbooks;

        }
        public IEnumerable<BorrowBooks> GetUsersWhoBorrowedBookbyBookName(string bookName)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];
            List<BorrowBooks> borrowedBooks = new List<BorrowBooks>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Add wildcards (%) to allow partial match in the LIKE query
                    string query = "SELECT BorrowedBooks.*, Books.BookName FROM BorrowedBooks " +
                                   "INNER JOIN Books ON BorrowedBooks.BookID = Books.BookID " +
                                   "WHERE Books.BookName LIKE @BookName";

                    // Create the SQL command and add the parameter with wildcard characters for partial match
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BookName", "%" + bookName + "%"); // Add wildcards for partial match

                    // Use SqlDataAdapter to fill the data table
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    // Open the connection and fill the data table with the result of the query
                    connection.Open();
                    dataAdapter.Fill(dataTable);

                    // Map data to BorrowBooks objects
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        BorrowBooks borrowBook = new BorrowBooks
                        {
                            BorrowID = Convert.ToInt32(dataRow["BorrowID"]),
                            UserID = Convert.ToInt32(dataRow["UserID"]),
                            BookID = Convert.ToInt32(dataRow["BookID"]),
                            BookName = dataRow["BookName"]?.ToString(),
                            BorrowDate = Convert.ToDateTime(dataRow["BorrowDate"]),
                            DueDate = Convert.ToDateTime(dataRow["DueDate"]),
                            ReturnDate = dataRow["ReturnDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dataRow["ReturnDate"]),
                            RenewalCount = Convert.ToInt32(dataRow["RenewalCount"]),
                            FineAmount = dataRow["FineAmount"] == DBNull.Value ? 0 : Convert.ToInt32(dataRow["FineAmount"]),
                            IsFinePaid = dataRow["IsFinePaid"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(dataRow["IsFinePaid"])
                        };
                        borrowedBooks.Add(borrowBook);
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception here if needed
                    throw new Exception("Error retrieving borrowed books data", ex);
                }
            }
            return borrowedBooks;
        }
        public void BorrowBook(BorrowBooks book)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Start a transaction to ensure atomic operations
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string checkBookQuery = @"SELECT COUNT(1) 
                                              FROM Books 
                                              WHERE BookID = @BookID";

                            using (SqlCommand checkBookCommand = new SqlCommand(checkBookQuery, connection, transaction))
                            {
                                checkBookCommand.Parameters.AddWithValue("@BookID", book.BookID);

                                int bookCount = (int)checkBookCommand.ExecuteScalar();

                                if (bookCount == 0)
                                {
                                    throw new InvalidOperationException("The specified book does not exist.");
                                }
                            }

                            // Check if the user exists in the Users table
                            string checkUserQuery = @"SELECT COUNT(1) 
                                              FROM Users 
                                              WHERE UserID = @UserID";

                            using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection, transaction))
                            {
                                checkUserCommand.Parameters.AddWithValue("@UserID", book.UserID);

                                int userCount = (int)checkUserCommand.ExecuteScalar();

                                if (userCount == 0)
                                {
                                    throw new InvalidOperationException("The specified user does not exist.");
                                }
                            }
                            // Check if the book is already borrowed and not returned
                            string checkQuery = @"SELECT COUNT(1) 
                                                  FROM BorrowedBooks 
                                                  WHERE BookID = @BookID AND ReturnDate IS NULL";

                            using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCommand.Parameters.AddWithValue("@BookID", book.BookID);

                                int count = (int)checkCommand.ExecuteScalar();

                                if (count > 0)
                                {
                                    throw new InvalidOperationException("This book has already been borrowed and has not been returned.");
                                }
                            }

                            string checkUserBooksQuery = @"SELECT COUNT(1) 
                                                           FROM BorrowedBooks 
                                                           WHERE UserID = @UserID AND ReturnDate IS NULL";

                            using (SqlCommand checkUserBooksCommand = new SqlCommand(checkUserBooksQuery, connection, transaction))
                            {
                                checkUserBooksCommand.Parameters.AddWithValue("@UserID", book.UserID);

                                int userBooksCount = (int)checkUserBooksCommand.ExecuteScalar();

                                if (userBooksCount >= 5)
                                {
                                    // If user has borrowed 5 or more books, throw an exception to prompt them to return one
                                    throw new InvalidOperationException("You have already borrowed 5 books. Please return one before borrowing another.");
                                }
                            }
                            // Proceed with borrowing the book
                            string query = @"INSERT INTO BorrowedBooks (UserID, BookID) 
                                            VALUES (@UserID, @BookID)

                                            Update Books
                                            SET IsAvailable =0
                                            Where BookID = @BookID";

                            using (SqlCommand command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@UserID", book.UserID);
                                command.Parameters.AddWithValue("@BookID", book.BookID);

                                command.ExecuteNonQuery();
                            }

                            // Commit the transaction if everything is successful
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // If any error occurs, roll back the transaction
                            transaction.Rollback();
                            // Log the exception
                            Console.WriteLine($"An error occurred while borrowing the book: {ex.Message}");
                            throw; // Rethrow the exception to handle it at the controller level
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (for now, we are printing it to the console)
                Console.WriteLine($"An error occurred while borrowing the book: {ex.Message}");
                throw; // Rethrow to be caught in the controller
            }
        }
        public void ReturnBook(BorrowBooks book)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Check if the book is currently borrowed by the user
                            string checkQuery = @"
                                                SELECT COUNT(1)
                                                FROM BorrowedBooks
                                                WHERE BookID = @BookID 
                                                AND UserID = @UserID 
                                                AND ReturnDate IS NULL";

                            using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCommand.Parameters.AddWithValue("@BookID", book.BookID);
                                checkCommand.Parameters.AddWithValue("@UserID", book.UserID);

                                int count = (int)checkCommand.ExecuteScalar();

                                if (count == 0)
                                {
                                    throw new InvalidOperationException("The book is not currently borrowed by this user.");
                                }
                            }

                            // Update the return date, fine amount, book availability, and user's total fine
                            string updateQuery = @"
                                                
                                                UPDATE BorrowedBooks
                                                SET ReturnDate = GETDATE(),
                                                    IsFinePaid = 0,
                                                    FineAmount = CASE 
                                                        WHEN DueDate < GETDATE() THEN DATEDIFF(DAY, DueDate, GETDATE()) * 1
                                                        ELSE 0
                                                     
                                                    END
                                                WHERE BookID = @BookID 
                                                AND UserID = @UserID 
                                                AND ReturnDate IS NULL;

                                                UPDATE Books
                                                SET IsAvailable = 1
                                                WHERE BookID = @BookID;

                                                UPDATE Users
                                                SET TotalFine = (
                                                    SELECT SUM(FineAmount) 
                                                    FROM BorrowedBooks 
                                                    WHERE UserID = @UserID AND IsPaidFine = 0
                                                )
                                                WHERE UserID = @UserID;
                                            ";

                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction))
                            {
                                updateCommand.Parameters.AddWithValue("@BookID", book.BookID);
                                updateCommand.Parameters.AddWithValue("@UserID", book.UserID);
                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    throw new InvalidOperationException("Failed to update the return status for the book.");
                                }
                            }

                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction in case of an error
                            transaction.Rollback();
                            Console.WriteLine($"An error occurred while returning the book: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error and rethrow it
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        void IBorrowAndReturnBooksDataAccessObject.RenewalBook(BorrowBooks book)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Check if the book is borrowed by the user
                            string checkQuery = @"
                                                SELECT COUNT(1)
                                                FROM BorrowedBooks
                                                WHERE BookID = @BookID 
                                                AND UserID = @UserID 
                                                AND ReturnDate IS NULL";

                            using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCommand.Parameters.AddWithValue("@BookID", book.BookID);
                                checkCommand.Parameters.AddWithValue("@UserID", book.UserID);

                                int count = (int)checkCommand.ExecuteScalar();

                                if (count == 0)
                                {
                                    throw new InvalidOperationException("The book is either not borrowed by this user or has already been returned.");
                                }
                            }

                            // Check if the current date is the due date or within the allowed renewal period (1-2 days before due date)
                            string renewalEligibilityQuery = @"
                                                            SELECT COUNT(1)
                                                            FROM BorrowedBooks
                                                            WHERE BookID = @BookID 
                                                            AND UserID = @UserID 
                                                            AND ReturnDate IS NULL
                                                            AND DueDate >= DATEADD(DAY, -2, GETDATE()) 
                                                            AND DueDate <= GETDATE()";

                            using (SqlCommand eligibilityCommand = new SqlCommand(renewalEligibilityQuery, connection, transaction))
                            {
                                eligibilityCommand.Parameters.AddWithValue("@BookID", book.BookID);
                                eligibilityCommand.Parameters.AddWithValue("@UserID", book.UserID);

                                int count = (int)eligibilityCommand.ExecuteScalar();

                                if (count == 0)
                                {
                                    throw new InvalidOperationException("The book can only be renewed on the due date or 1-2 days before it. Please check the due date.");
                                }
                            }

                            // Extend the due date for the borrowed book (e.g., extending by 30 days)
                            string extendDueDateQuery = @"
                                                        UPDATE BorrowedBooks
                                                        SET DueDate = DATEADD(DAY, 30, DueDate) 
                                                        WHERE BookID = @BookID 
                                                        AND UserID = @UserID 
                                                        AND ReturnDate IS NULL";

                            using (SqlCommand extendDueDateCommand = new SqlCommand(extendDueDateQuery, connection, transaction))
                            {
                                extendDueDateCommand.Parameters.AddWithValue("@BookID", book.BookID);
                                extendDueDateCommand.Parameters.AddWithValue("@UserID", book.UserID);
                                int rowsAffected = extendDueDateCommand.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    throw new InvalidOperationException("Failed to extend the due date. The book may not be eligible for renewal.");
                                }
                            }

                            // Commit the transaction if everything is successful
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction in case of an error
                            transaction.Rollback();
                            Console.WriteLine($"An error occurred while renewing the book: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error and rethrow it
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        void IBorrowAndReturnBooksDataAccessObject.FinePayment(FinePayments fines)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Start a transaction to ensure atomic operations
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Check if the user exists and has an outstanding fine
                            string checkUserFineQuery = @"
                                                        SELECT TotalFine
                                                        FROM Users
                                                        WHERE UserID = @UserID";

                            using (SqlCommand checkUserFineCommand = new SqlCommand(checkUserFineQuery, connection, transaction))
                            {
                                checkUserFineCommand.Parameters.AddWithValue("@UserID", fines.UserID);

                                object totalFineObj = checkUserFineCommand.ExecuteScalar();

                                if (totalFineObj == null)
                                {
                                    throw new InvalidOperationException("The specified user does not exist.");
                                }

                                decimal totalFine = (decimal)totalFineObj;

                                if (totalFine <= 0)
                                {
                                    throw new InvalidOperationException("The user does not have any outstanding fines.");
                                }

                                if (fines.Amount > totalFine)
                                {
                                    throw new InvalidOperationException("The payment amount exceeds the user's total fine.");
                                }
                                if (fines.Amount < totalFine)
                                {
                                    throw new InvalidOperationException("The user must clear All the total fine.");
                                }
                            }

                            // Deduct the fine payment from the user's total fine
                            string updateUserFineQuery = @"
                                                         UPDATE Users
                                                         SET TotalFine = TotalFine - @Amount
                                                         WHERE UserID = @UserID

                                                         Update BorrowedBooks
                                                         SET IsFinePaid =1
                                                         Where UserID = @UserID";

                            using (SqlCommand updateUserFineCommand = new SqlCommand(updateUserFineQuery, connection, transaction))
                            {
                                updateUserFineCommand.Parameters.AddWithValue("@UserID", fines.UserID);
                                updateUserFineCommand.Parameters.AddWithValue("@Amount", fines.Amount);
                                updateUserFineCommand.ExecuteNonQuery();
                            }

                            // Record the fine payment in the FinePayments table
                            string insertFinePaymentQuery = @"
                                                            INSERT INTO FinePayments (UserID, Amount, PaymentDate, PaymentMethod, TransactionId)
                                                            VALUES (@UserID, @Amount, GETDATE(), @PaymentMethod, @TransactionID)";

                            using (SqlCommand insertFinePaymentCommand = new SqlCommand(insertFinePaymentQuery, connection, transaction))
                            {
                                insertFinePaymentCommand.Parameters.AddWithValue("@UserID", fines.UserID);
                                insertFinePaymentCommand.Parameters.AddWithValue("@Amount", fines.Amount);
                                insertFinePaymentCommand.Parameters.AddWithValue("@PaymentMethod",fines.PaymentMethod);
                                insertFinePaymentCommand.Parameters.AddWithValue("@TransactionID", fines.TransactionID);
                                insertFinePaymentCommand.ExecuteNonQuery();
                            }

                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Roll back the transaction in case of an error
                            transaction.Rollback();
                            Console.WriteLine($"An error occurred during fine payment: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        IEnumerable<FinePayments> IBorrowAndReturnBooksDataAccessObject.GetAllPayments()
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];
            List<FinePayments> Fines = new List<FinePayments>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"SELECT * from FinePayments";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();

                // Fill the data table with the result of the query
                dataAdapter.Fill(dataTable);

                // Convert the data rows into BorrowBooks objects
                foreach (DataRow datarow in dataTable.Rows)
                {
                    FinePayments Fine = new FinePayments
                    {
                        PaymentID = Convert.ToInt32(datarow["PaymentID"]),
                        UserID = Convert.ToInt32(datarow["UserID"]),
                        Amount = Convert.ToDecimal(datarow["Amount"]),
                        PaymentDate = Convert.ToDateTime(datarow["PaymentDate"]),
                        PaymentMethod = Convert.ToString(datarow["PaymentMethodCount"]),
                        TransactionID = Convert.ToString(datarow["TransactionID"])
                    };
                    Fines.Add(Fine);
                }
            }

            return Fines;
        }

        IEnumerable<FinePayments> IBorrowAndReturnBooksDataAccessObject.GetPayment(int id)
        {
            string connectionString = configuration["ConnectionStrings:DefaultConnection"];
            List<FinePayments> Fines = new List<FinePayments>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"SELECT * from FinePayments Where UserID = {id}";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();

                // Fill the data table with the result of the query
                dataAdapter.Fill(dataTable);

                // Convert the data rows into BorrowBooks objects
                foreach (DataRow datarow in dataTable.Rows)
                {
                    FinePayments Fine = new FinePayments
                    {
                        PaymentID = Convert.ToInt32(datarow["PaymentID"]),
                        UserID = Convert.ToInt32(datarow["UserID"]),
                        Amount = Convert.ToDecimal(datarow["Amount"]),
                        PaymentDate = Convert.ToDateTime(datarow["PaymentDate"]),
                        PaymentMethod = Convert.ToString(datarow["PaymentMethodCount"]),
                        TransactionID = Convert.ToString(datarow["TransactionID"])
                    };
                    Fines.Add(Fine);
                }
            }

            return Fines;
        }
    }
}
