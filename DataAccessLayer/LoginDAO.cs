using System.Data.SqlClient;
using LibraryManagementSystem.Models;
namespace LibraryManagementSystem.DataAccessLayer
{
    public class LoginDAO : ILoginDAO
    {
        private readonly IConfiguration _configuration;

        public LoginDAO(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Login AuthenticateUser(string userName, string password)
        {
            string connectionString = _configuration.GetConnectionString("LMSConnection");
            Login login = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Ensure the Password column is compared in a case-sensitive manner
                string query = "SELECT * FROM Login WHERE UserName = @UserName COLLATE Latin1_General_BIN AND Password = @Password COLLATE Latin1_General_BIN";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserName", userName);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();  // Open the connection synchronously
                SqlDataReader reader = command.ExecuteReader();  // Execute the reader synchronously

                if (reader.HasRows)
                {
                    reader.Read();  // Read the data
                    login = new Login
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        UserName = reader["UserName"].ToString(),
                        Password = reader["Password"].ToString(),
                        Role = reader["Role"].ToString()  // Fetch the role
                    };
                }

                reader.Close();  // Close the reader
            }

            return login;
        }

        // Validate if username and userID match
        public Login ValidateUserForPasswordReset(string userName, int userID)
        {
            string connectionString = _configuration.GetConnectionString("LMSConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT UserID, UserName FROM Users WHERE UserName = @UserName AND UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@UserID", userID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Login
                        {
                            UserID = reader.GetInt32(0),
                            UserName = reader.GetString(1)
                        };
                    }
                }
            }
            return null; // No match found
        }
        // Update the password for the given username
        public bool UpdatePassword(string userName, string newPassword)
        {
            string connectionString = _configuration.GetConnectionString("LMSConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "UPDATE Login SET Password = @Password WHERE UserName = @UserName";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Password", newPassword); // Note: You should hash the password in a real system
                cmd.Parameters.AddWithValue("@UserName", userName);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0; // Return true if the password was updated successfully
            }
        }
    }
}
