using LibraryManagementSystem.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace LibraryManagementSystem.DataAccessLayer
{
    public class UsersDAO : IUsersDAO
    {
        public IConfiguration Configuration { get; }

        public UsersDAO(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Get all users
        public IEnumerable<Users> GetAllUsers()
        {
            string connectionString = Configuration["ConnectionStrings:LMSConnection"];
            List<Users> usersList = new List<Users>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Users ";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    Users user = new Users();
                    user.UserID = Convert.ToInt32(row[0]);
                    user.UserName = row[1].ToString();
                    user.Age = Convert.ToInt32(row[2]);
                    user.PhoneNumber = row[3].ToString();
                    user.Email = row[4].ToString();
                    user.TotalFine = Convert.ToInt32(row[5]);
                    user.IsActive = Convert.ToBoolean(row[6]);

                    usersList.Add(user);
                }
            }
            return usersList;
        }
        public IEnumerable<Users> GetActiveUsers()
        {
            string connectionString = Configuration["ConnectionStrings:LMSConnection"];
            List<Users> usersList = new List<Users>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Users WHERE IsActive = 1";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    Users user = new Users();
                    user.UserID = Convert.ToInt32(row[0]);
                    user.UserName = row[1].ToString();
                    user.Age = Convert.ToInt32(row[2]);
                    user.PhoneNumber = row[3].ToString();
                    user.Email = row[4].ToString();
                    user.TotalFine = Convert.ToInt32(row[5]);
                    user.IsActive = Convert.ToBoolean(row[6]);

                    usersList.Add(user);
                }
            }
            return usersList;
        }

        // Add a new user
        public void AddUser(Users user, Login login)
        {
            bool existingUser = IsUserNameExists(user.UserName);

            if (existingUser == true)
            {
                // Username already exists and is active
                throw new Exception("Username already exists. Please choose a different username.");
            }

            string connectionString = Configuration["ConnectionStrings:LMSConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                // Insert user and get the UserID
                string uQuery = "INSERT INTO Users (UserName, Age, PhoneNumber, Email, TotalFine) " +
                                "VALUES (@UserName, @Age, @PhoneNumber, @Email, @TotalFine);" +
                                "SELECT SCOPE_IDENTITY();";  // Get the UserID

                SqlCommand uCommand = new SqlCommand(uQuery, connection);
                uCommand.Parameters.AddWithValue("@UserName", user.UserName);
                uCommand.Parameters.AddWithValue("@Age", user.Age);
                uCommand.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                uCommand.Parameters.AddWithValue("@Email", user.Email);
                uCommand.Parameters.AddWithValue("@TotalFine", user.TotalFine);

                // Get the UserID
                user.UserID = Convert.ToInt32(uCommand.ExecuteScalar());
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Insert user and get the UserID
                string lQuery = "INSERT INTO Login (UserID, UserName, Password, Role) " +
                               "VALUES (@UserID, @UserName, @Password, @Role);" +
                               "SELECT SCOPE_IDENTITY();";

                SqlCommand lCommand = new SqlCommand(lQuery, connection);
                lCommand.Parameters.AddWithValue("@UserID", user.UserID);
                lCommand.Parameters.AddWithValue("@UserName", user.UserName);
                lCommand.Parameters.AddWithValue("@Password", login.Password);
                lCommand.Parameters.AddWithValue("@Role", login.Role);

                lCommand.ExecuteNonQuery();
            }
        }

        // Check if username exists before adding
        public bool IsUserNameExists(string userName)
        {
            string connectionString = Configuration["ConnectionStrings:LMSConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Users WHERE UserName = @UserName";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserName", userName);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Check if the reader has any rows, indicating the username exists
                if (reader.HasRows)
                {
                    reader.Read();
                    return true; // Username exists
                }
                else
                {
                    return false; // Username does not exist
                }
            }
        }


        public Users ReactivateUser(int userID)
        {
            string connectionString = Configuration["ConnectionStrings:LMSConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Reactivate the user by updating the IsActive field
                string query = "UPDATE Users SET IsActive = 1 WHERE UserID = @UserID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userID);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Fetch the updated user after reactivation
                    string fetchQuery = "SELECT * FROM Users WHERE UserID = @UserID";
                    SqlCommand fetchCommand = new SqlCommand(fetchQuery, connection);
                    fetchCommand.Parameters.AddWithValue("@UserID", userID);

                    SqlDataReader reader = fetchCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new Users
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            UserName = reader["UserName"].ToString(),
                            Age = Convert.ToInt32(reader["Age"]),
                            PhoneNumber = reader["PhoneNumber"].ToString(),
                            Email = reader["Email"].ToString(),
                            TotalFine = Convert.ToInt32(reader["TotalFine"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                }

                return null; // If user is not found, return null
            }
        }





        // Get a single user by UserID
        public Users GetUser(int UserID)
        {
            string connectionString = Configuration["ConnectionStrings:LMSConnection"];
            Users user = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT UserID, UserName, Age, PhoneNumber, TotalFine, Email, IsActive FROM Users WHERE UserID = @UserID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    user = new Users
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        UserName = reader["UserName"].ToString(),
                        Age = Convert.ToInt32(reader["Age"]),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        TotalFine = Convert.ToDecimal(reader["TotalFine"]),
                        Email = reader["Email"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                    };
                }
                reader.Close();
            }

            if (user == null)
            {
                throw new Exception($"{UserID} userid doesn't exists.");  // Throw exception if user is not found
            }
            return user;
        }

        public IEnumerable<Users> GetUser(string UserName)
        {
            string connectionString = Configuration["ConnectionStrings:LMSConnection"];
            List<Users> usersList = new List<Users>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Using LIKE for partial matching
                string query = "SELECT UserID, UserName, Age, PhoneNumber, TotalFine, Email, IsActive " +
                               "FROM Users WHERE UserName LIKE @UserName";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = "%" + UserName + "%";

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Loop through all matching rows
                while (reader.Read())
                {
                    Users user = new Users
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        UserName = reader["UserName"].ToString(),
                        Age = Convert.ToInt32(reader["Age"]),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        TotalFine = Convert.ToDecimal(reader["TotalFine"]),
                        Email = reader["Email"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                    };

                    // Add each matching user to the list
                    usersList.Add(user);
                }
                reader.Close();
            }

            // If no users were found, throw an exception
            if (usersList.Count == 0)
            {
                throw new Exception($"No users found with a username containing '{UserName}'.");
            }

            return usersList;
        }





        // Delete a user (soft delete and archive)
        public void DeleteUser(int UserID)
        {
            Users user = GetUser(UserID); // This will throw an exception if UserID is invalid
            if (user != null)
            {
                // Check if the user has unpaid fines
                if (user.TotalFine > 0)
                {
                    throw new Exception("The user has an outstanding fine. Please make sure pending fines are cleared.");
                }

                // Check if the user has borrowed books
                if (HasBorrowedBooks(UserID))
                {
                    throw new Exception("The user has borrowed books.So, you can't delete the user.");
                }
                if (!user.IsActive)
                {
                    throw new Exception($"User with ID {UserID} is inactive.");  // Throw exception if user is inactive
                }

                string connectionString = Configuration["ConnectionStrings:LMSConnection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID"; // Set IsActive to false
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserID", UserID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                throw new Exception($"User with ID {UserID} deleted successfully.");
            }
            else
            {
                throw new Exception($"Invalid UserID: {UserID}"); // If the user does not exist, throw exception
            }
        }
        private bool HasBorrowedBooks(int userID)
        {
            string connectionString = Configuration["ConnectionStrings:LMSConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM BorrowedBooks WHERE UserID = @UserID AND ReturnDate IS NULL";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userID);

                connection.Open();
                int borrowedCount = Convert.ToInt32(command.ExecuteScalar());

                return borrowedCount > 0; // Return true if the user has borrowed books that are not returned
            }
        }


        public void EditUser(int UserID, Users user)
        {
            // Fetch the existing user details
            Users existingUser = GetUser(UserID);

            // Check if the user exists
            if (existingUser == null)
            {
                throw new Exception($"User with ID {UserID} does not exist.");
            }

            // Set the updated fields (if provided), otherwise keep the existing values
            string updatedUserName = string.IsNullOrEmpty(user.UserName) ? existingUser.UserName : user.UserName;
            int updatedAge = user.Age == 0 ? existingUser.Age : user.Age;  // Assuming Age is a non-nullable int
            string updatedPhoneNumber = string.IsNullOrEmpty(user.PhoneNumber) ? existingUser.PhoneNumber : user.PhoneNumber;
            string updatedEmail = string.IsNullOrEmpty(user.Email) ? existingUser.Email : user.Email;

            // If user has provided an IsActive value, use it; otherwise, keep the old value
            bool updatedIsActive = user.IsActive != existingUser.IsActive ? user.IsActive : existingUser.IsActive;

            // Prepare the SQL query to update the user details
            string connectionString = Configuration["ConnectionStrings:LMSConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Users SET UserName = @UserName, Age = @Age, PhoneNumber = @PhoneNumber, Email = @Email, " +
                    "IsActive = @IsActive WHERE UserID = @UserID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserName", updatedUserName);
                command.Parameters.AddWithValue("@Age", updatedAge);
                command.Parameters.AddWithValue("@PhoneNumber", updatedPhoneNumber);
                command.Parameters.AddWithValue("@Email", updatedEmail);
                command.Parameters.AddWithValue("@IsActive", updatedIsActive);
                command.Parameters.AddWithValue("@UserID", UserID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
