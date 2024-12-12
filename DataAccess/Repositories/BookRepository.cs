using System.Data.SqlClient;
using LibraryAPI.DataAccess.Interfaces;
using LibraryAPI.DataAccess.Models;
using LibraryAPI.DataAccess.Data;

namespace LibraryAPI.DataAccess.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly DatabaseHelper _databaseHelper;

        public BookRepository(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public bool AddBook(Book book, string authorName, string genreName, string publisherName)
        {
            using (var connection = _databaseHelper.CreateConnection())
            {
                var sqlConnection = (SqlConnection)connection;
                sqlConnection.Open();

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        // Insert the book and get its new ID (even if the book already exists)
                        var insertBookCommand = new SqlCommand(
                            "INSERT INTO Books (BookName, BookPrice) VALUES (@Name, @Price); SELECT SCOPE_IDENTITY();",
                            sqlConnection,
                            transaction
                        );
                        insertBookCommand.Parameters.AddWithValue("@Name", book.BookName);
                        insertBookCommand.Parameters.AddWithValue("@Price", book.BookPrice);

                        int bookId = Convert.ToInt32(insertBookCommand.ExecuteScalar());

                        // Insert related entities and get their IDs (insert even if already present)
                        int authorId = InsertOrGetId(sqlConnection, transaction, "Authors", "AuthorName", authorName, true);
                        int genreId = InsertOrGetId(sqlConnection, transaction, "Genres", "GenreName", genreName, true);
                        int publisherId = InsertOrGetId(sqlConnection, transaction, "Publishers", "PublisherName", publisherName, true);

                        // Insert mappings for the relationships (with the new book ID)
                        InsertMapping(sqlConnection, transaction, "BooksAuthorsMap", "BookID", "AuthorID", bookId, authorId);
                        InsertMapping(sqlConnection, transaction, "BooksGenresMap", "BookID", "GenreID", bookId, genreId);
                        InsertMapping(sqlConnection, transaction, "BooksPublishersMap", "BookID", "PublisherID", bookId, publisherId);

                        // Commit transaction
                        transaction.Commit();

                        return true; // Success
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Log the full error for debugging
                        Console.WriteLine($"Error during AddBook: {ex.Message}\n{ex.StackTrace}");
                        return false; // Failure
                    }
                }
            }
        }


        public int InsertOrGetId(
            SqlConnection connection,
            SqlTransaction transaction,
            string tableName,
            string columnName,
            string value,
            bool forceInsert = false)  // New parameter to force insertion
        {
            if (forceInsert)
            {
                // Insert the value into the table and return the newly generated ID
                var insertCommand = new SqlCommand(
                    $"INSERT INTO {tableName} ({columnName}) VALUES (@Value); SELECT SCOPE_IDENTITY();",
                    connection,
                    transaction
                );
                insertCommand.Parameters.AddWithValue("@Value", value);

                var newId = insertCommand.ExecuteScalar();
                return newId != DBNull.Value ? Convert.ToInt32(newId) : 0; // Return 0 if insertion fails
            }
            else
            {
                // Check if the value already exists in the table
                var checkExistsCommand = new SqlCommand(
                    $"SELECT {columnName} FROM {tableName} WHERE {columnName} = @Value;",
                    connection,
                    transaction
                );
                checkExistsCommand.Parameters.AddWithValue("@Value", value);

                var existingId = checkExistsCommand.ExecuteScalar();

                if (existingId != null && existingId != DBNull.Value)
                {
                    // If the value already exists, return the existing ID
                    return Convert.ToInt32(existingId);
                }

                // Insert the value into the table and return the newly generated ID
                var insertCommand = new SqlCommand(
                    $"INSERT INTO {tableName} ({columnName}) VALUES (@Value); SELECT SCOPE_IDENTITY();",
                    connection,
                    transaction
                );
                insertCommand.Parameters.AddWithValue("@Value", value);

                var newId = insertCommand.ExecuteScalar();
                return newId != DBNull.Value ? Convert.ToInt32(newId) : 0; // Return 0 if insertion fails
            }
        }


        public void InsertMapping(
            SqlConnection connection,
            SqlTransaction transaction,
            string tableName,
            string bookColumn,
            string relatedColumn,
            int bookId,
            int relatedId)
        {
            // Insert the mapping into the table
            var insertMappingCommand = new SqlCommand(
                $"INSERT INTO {tableName} ({bookColumn}, {relatedColumn}) VALUES (@BookId, @RelatedId);",
                connection,
                transaction
            );
            insertMappingCommand.Parameters.AddWithValue("@BookId", bookId);
            insertMappingCommand.Parameters.AddWithValue("@RelatedId", relatedId);

            insertMappingCommand.ExecuteNonQuery();
        }



        public IEnumerable<Book> GetAllBooks()
        {
            var books = new List<Book>();

            using (var connection = _databaseHelper.CreateConnection())
            {
                connection.Open();

                var query = @"
            SELECT 
                b.BookID, 
                b.BookName, 
                b.BookPrice, 
                a.AuthorName, 
                g.GenreName, 
                p.PublisherName
            FROM Books b
            LEFT JOIN BooksAuthorsMap bam ON b.BookID = bam.BookID
            LEFT JOIN Authors a ON bam.AuthorID = a.AuthorID
            LEFT JOIN BooksGenresMap bgm ON b.BookID = bgm.BookID
            LEFT JOIN Genres g ON bgm.GenreID = g.GenreID
            LEFT JOIN BooksPublishersMap bpm ON b.BookID = bpm.BookID
            LEFT JOIN Publishers p ON bpm.PublisherID = p.PublisherID";

                var command = new SqlCommand(query, (SqlConnection)connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new Book
                        {
                            BookId = reader.GetInt32(0),
                            BookName = reader.GetString(1),
                            BookPrice = reader.GetDecimal(2),
                            Authors = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Genres = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Publishers = reader.IsDBNull(5) ? null : reader.GetString(5)
                        };

                        books.Add(book);
                    }
                }
            }

            return books;
        }




        public void UpdateBook(int bookId, string? bookName = null, decimal? bookPrice = null, string? authorName = null, string? genreName = null, string? publisherName = null)
        {
            if (bookId <= 0)
            {
                throw new ArgumentException("Invalid book ID.");
            }

            using (var connection = _databaseHelper.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        bool isUpdateRequired = false;

                        // Update the Books table if bookName or bookPrice is provided
                        if (bookName != null || bookPrice.HasValue)
                        {
                            var updateBookQuery = "UPDATE Books SET ";
                            var parameters = new List<SqlParameter>();

                            if (bookName != null)
                            {
                                updateBookQuery += "BookName = @BookName, ";
                                parameters.Add(new SqlParameter("@BookName", bookName));
                            }
                            if (bookPrice.HasValue)
                            {
                                updateBookQuery += "BookPrice = @BookPrice, ";
                                parameters.Add(new SqlParameter("@BookPrice", bookPrice.Value));
                            }

                            // Finalize the query by trimming the trailing comma and adding the WHERE clause
                            updateBookQuery = updateBookQuery.TrimEnd(',', ' ') + " WHERE BookID = @BookId";
                            parameters.Add(new SqlParameter("@BookId", bookId));

                            // Execute the query
                            var command = new SqlCommand(updateBookQuery, (SqlConnection)connection, (SqlTransaction)transaction);
                            command.Parameters.AddRange(parameters.ToArray());
                            command.ExecuteNonQuery();

                            isUpdateRequired = true;
                        }

                        // Update mappings for Author, Genre, Publisher if provided
                        if (authorName != null)
                        {
                            UpdateMapping("Authors", "AuthorName", "BooksAuthorsMap", "AuthorID", authorName, bookId, (SqlConnection)connection, (SqlTransaction)transaction);
                            isUpdateRequired = true;
                        }
                        if (genreName != null)
                        {
                            UpdateMapping("Genres", "GenreName", "BooksGenresMap", "GenreID", genreName, bookId, (SqlConnection)connection, (SqlTransaction)transaction);
                            isUpdateRequired = true;
                        }
                        if (publisherName != null)
                        {
                            UpdateMapping("Publishers", "PublisherName", "BooksPublishersMap", "PublisherID", publisherName, bookId, (SqlConnection)connection, (SqlTransaction)transaction);
                            isUpdateRequired = true;
                        }

                        // Ensure that at least one update was performed
                        if (!isUpdateRequired)
                        {
                            throw new ArgumentException("No fields provided to update.");
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error updating book: {ex.Message}");
                        throw;
                    }
                }
            }
        }


        // Helper method for updating mappings (Author, Genre, Publisher)
        private void UpdateMapping(
        string tableName,
        string columnName,
        string mappingTableName,
        string foreignKeyColumn,
        string value,
        int bookId,
        SqlConnection connection,
        SqlTransaction transaction)
        {
            // Find the ID corresponding to the provided value in the related table
            var getIdQuery = $"SELECT {foreignKeyColumn} FROM {tableName} WHERE {columnName} = @Value";
            var command = new SqlCommand(getIdQuery, connection, transaction);
            command.Parameters.AddWithValue("@Value", value);
            var id = command.ExecuteScalar();

            if (id == null)
            {
                // If the value doesn't exist in the table, insert it
                var insertQuery = $"INSERT INTO {tableName} ({columnName}) OUTPUT INSERTED.{foreignKeyColumn} VALUES (@Value)";
                command = new SqlCommand(insertQuery, connection, transaction);
                command.Parameters.AddWithValue("@Value", value);
                id = command.ExecuteScalar();
            }

            // Update the mapping table to link the book with the new ID
            var updateMappingQuery = $"UPDATE {mappingTableName} SET {foreignKeyColumn} = @Id WHERE BookID = @BookId";
            command = new SqlCommand(updateMappingQuery, connection, transaction);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@BookId", bookId);
            command.ExecuteNonQuery();
        }



        public bool DeleteBook(int bookId)
        {
            using (var connection = _databaseHelper.CreateConnection())
            {
                connection.Open();

                // Check if the book exists before attempting to delete
                var checkQuery = "SELECT COUNT(*) FROM Books WHERE BookID = @Id";
                var checkCommand = new SqlCommand(checkQuery, (SqlConnection)connection);
                checkCommand.Parameters.AddWithValue("@Id", bookId);

                // Execute the query and check the result
                int bookExists = (int)checkCommand.ExecuteScalar();

                if (bookExists == 0)
                {
                    // If the book doesn't exist, log it and return false
                    throw new Exception($"Book with ID {bookId} was not found in the database.");
                }

                // Proceed to delete the book if it exists
                var deleteQuery = "DELETE FROM Books WHERE BookID = @Id";
                var deleteCommand = new SqlCommand(deleteQuery, (SqlConnection)connection);
                deleteCommand.Parameters.AddWithValue("@Id", bookId);

                int rowsAffected = deleteCommand.ExecuteNonQuery();

                // If no rows were affected, return false
                if (rowsAffected == 0)
                {
                    return false; // Deletion failed for some reason
                }

                // Delete the Author if it's not associated with any other book
                var deleteAuthorQuery = @"
            DELETE FROM Authors
            WHERE AuthorID NOT IN (
                SELECT AuthorID
                FROM BooksAuthorsMap
                WHERE BookID != @Id
            )";
                var deleteAuthorCommand = new SqlCommand(deleteAuthorQuery, (SqlConnection)connection);
                deleteAuthorCommand.Parameters.AddWithValue("@Id", bookId);
                deleteAuthorCommand.ExecuteNonQuery();

                // Delete the Genre if it's not associated with any other book
                var deleteGenreQuery = @"
            DELETE FROM Genres
            WHERE GenreID NOT IN (
                SELECT GenreID
                FROM BooksGenresMap
                WHERE BookID != @Id
            )";
                var deleteGenreCommand = new SqlCommand(deleteGenreQuery, (SqlConnection)connection);
                deleteGenreCommand.Parameters.AddWithValue("@Id", bookId);
                deleteGenreCommand.ExecuteNonQuery();

                // Delete the Publisher if it's not associated with any other book
                var deletePublisherQuery = @"
            DELETE FROM Publishers
            WHERE PublisherID NOT IN (
                SELECT PublisherID
                FROM BooksPublishersMap
                WHERE BookID != @Id
            )";
                var deletePublisherCommand = new SqlCommand(deletePublisherQuery, (SqlConnection)connection);
                deletePublisherCommand.Parameters.AddWithValue("@Id", bookId);
                deletePublisherCommand.ExecuteNonQuery();

                // Book successfully deleted
                return true;
            }
        }



        public IEnumerable<Book> SearchBooks(string searchBy, string searchValue)
        {
            var books = new List<Book>();

            using (var connection = _databaseHelper.CreateConnection())
            {
                connection.Open();

                // Map searchBy to the respective table and column
                string query = @"
        SELECT 
            b.BookID, 
            b.BookName, 
            b.BookPrice, 
            ISNULL(STRING_AGG(a.AuthorName, ', '), '') AS Authors, 
            ISNULL(STRING_AGG(g.GenreName, ', '), '') AS Genres, 
            ISNULL(STRING_AGG(p.PublisherName, ', '), '') AS Publishers
        FROM Books b
        LEFT JOIN BooksAuthorsMap bam ON b.BookID = bam.BookID
        LEFT JOIN Authors a ON bam.AuthorID = a.AuthorID
        LEFT JOIN BooksGenresMap bgm ON b.BookID = bgm.BookID
        LEFT JOIN Genres g ON bgm.GenreID = g.GenreID
        LEFT JOIN BooksPublishersMap bpm ON b.BookID = bpm.BookID
        LEFT JOIN Publishers p ON bpm.PublisherID = p.PublisherID
    ";

                // Append WHERE condition based on searchBy
                switch (searchBy.ToLower())
                {
                    case "authors":
                        query += " WHERE a.AuthorName LIKE '%' + @Value + '%'";
                        break;
                    case "genres":
                        query += " WHERE g.GenreName LIKE '%' + @Value + '%'";
                        break;
                    case "publishers":
                        query += " WHERE p.PublisherName LIKE '%' + @Value + '%'";
                        break;
                    case "bookid":
                        query += " WHERE b.BookID = @Value";
                        break;
                    case "bookprice":
                        query += " WHERE b.BookPrice = @Value";
                        break;
                    default:
                        query += " WHERE b.BookName LIKE '%' + @Value + '%'";
                        break;
                }

                query += @"
        GROUP BY b.BookID, b.BookName, b.BookPrice";

                var command = new SqlCommand(query, (SqlConnection)connection);

                // Use appropriate parameter type based on searchBy
                if (searchBy.ToLower() == "bookid")
                {
                    command.Parameters.AddWithValue("@Value", int.Parse(searchValue));
                }
                else if (searchBy.ToLower() == "bookprice")
                {
                    command.Parameters.AddWithValue("@Value", decimal.Parse(searchValue));
                }
                else
                {
                    command.Parameters.AddWithValue("@Value", searchValue);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            BookId = reader.GetInt32(0),
                            BookName = reader.GetString(1),
                            BookPrice = reader.GetDecimal(2),
                            Authors = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Genres = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Publishers = reader.IsDBNull(5) ? null : reader.GetString(5)
                        });
                    }
                }
            }

            return books;
        }




        public Book GetBookWithId(int bookId)
        {
            using (var connection = _databaseHelper.CreateConnection())
            {
                connection.Open();

                var command = new SqlCommand(@"
                SELECT 
                    b.BookID, b.BookName, b.BookPrice,
                    STRING_AGG(a.AuthorName, ', ') AS Authors,
                    STRING_AGG(g.GenreName, ', ') AS Genres,
                    STRING_AGG(p.PublisherName, ', ') AS Publishers
                FROM Books b
                LEFT JOIN BooksAuthorsMap bam ON b.BookID = bam.BookID
                LEFT JOIN Authors a ON bam.AuthorID = a.AuthorID
                LEFT JOIN BooksGenresMap bgm ON b.BookID = bgm.BookID
                LEFT JOIN Genres g ON bgm.GenreID = g.GenreID
                LEFT JOIN BooksPublishersMap bpm ON b.BookID = bpm.BookID
                LEFT JOIN Publishers p ON bpm.PublisherID = p.PublisherID
                WHERE b.BookID = @BookID
                GROUP BY b.BookID, b.BookName, b.BookPrice", (SqlConnection)connection);
                command.Parameters.AddWithValue("@BookID", bookId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Book
                        {
                            BookId = reader.GetInt32(0),
                            BookName = reader.GetString(1),
                            BookPrice = reader.GetDecimal(2),
                            Authors = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Genres = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Publishers = reader.IsDBNull(5) ? null : reader.GetString(5)
                        };
                    }
                }
            }

            return null;
        }










    }
}
