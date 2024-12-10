using System.Data;
using System.Data.SqlClient;

namespace LibraryAPI.DataAccess.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public object ConnectionString { get; internal set; }

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IDbCommand CreateCommand(string query, IDbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            return command;
        }
    }
}
