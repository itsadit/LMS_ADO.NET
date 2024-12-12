using System.Data.SqlClient;
using System.IO;

namespace LibraryAPI.DataAccess.Data
{
    public class MigrationHelper
    {
        private readonly string _connectionString;

        public MigrationHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ApplyMigration(string migrationScriptPath)
        {
            string script = File.ReadAllText(migrationScriptPath);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(script, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
