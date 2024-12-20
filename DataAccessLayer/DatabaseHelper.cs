using System.Data;
using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem.DataAccessLayer
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        // Constructor to initialize connection string
        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to create a new database connection
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Method to create a new database command
        public IDbCommand CreateCommand(string query, IDbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            return command;
        }
    }
}
