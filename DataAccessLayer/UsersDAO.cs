using LibraryManagementSystem.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem.DataAccessLayer
{
    public class UsersDAO : IUsersDAO
    {
        public IConfiguration Configuration { get; }
        private readonly DatabaseHelper _databaseHelper;

        public UsersDAO(IConfiguration configuration)
        {
            Configuration = configuration;
            _databaseHelper = new DatabaseHelper(Configuration["ConnectionStrings:LMSConnection"]);
        }

        // Get all users
        public IEnumerable<Users> GetAllUsers()
        {
            List<Users> usersList = new List<Users>();
            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                string query = "SELECT * FROM Users";
                IDbCommand command = _databaseHelper.CreateCommand(query, connection);
                connection.Open();
                IDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Users user = MapUser(reader); // Using the helper method
                    usersList.Add(user);
                }
            }
            return usersList;
        }

        // Get active users
        public IEnumerable<Users> GetActiveUsers()
        {
            List<Users> usersList = new List<Users>();
            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                string query = "SELECT * FROM Users WHERE IsActive = 1";
                IDbCommand command = _databaseHelper.CreateCommand(query, connection);
                connection.Open();
                IDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Users user = MapUser(reader); // Using the helper method
                    usersList.Add(user);
                }
            }
            return usersList;
        }

        // Add a new user
        public void AddUser(Users user, Login login)
        {
            if (IsUserNameExists(user.UserName))
            {
                throw new Exception("Username already exists. Please choose a different username.");
            }

            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                connection.Open();

                // Insert user and get the UserID
                string uQuery = "INSERT INTO Users (UserName, Age, PhoneNumber, Email, TotalFine) " +
                                "VALUES (@UserName, @Age, @PhoneNumber, @Email, @TotalFine);" +
                                "SELECT SCOPE_IDENTITY();";

                IDbCommand uCommand = _databaseHelper.CreateCommand(uQuery, connection);
                uCommand.Parameters.Add(new SqlParameter("@UserName", user.UserName));
                uCommand.Parameters.Add(new SqlParameter("@Age", user.Age));
                uCommand.Parameters.Add(new SqlParameter("@PhoneNumber", user.PhoneNumber));
                uCommand.Parameters.Add(new SqlParameter("@Email", user.Email));
                uCommand.Parameters.Add(new SqlParameter("@TotalFine", user.TotalFine));

                user.UserID = Convert.ToInt32(uCommand.ExecuteScalar());
            }

            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                connection.Open();

                // Insert login details
                string lQuery = "INSERT INTO Login (UserID, UserName, Password, Role) " +
                               "VALUES (@UserID, @UserName, @Password, @Role);";

                IDbCommand lCommand = _databaseHelper.CreateCommand(lQuery, connection);
                lCommand.Parameters.Add(new SqlParameter("@UserID", user.UserID));
                lCommand.Parameters.Add(new SqlParameter("@UserName", user.UserName));
                lCommand.Parameters.Add(new SqlParameter("@Password", login.Password));
                lCommand.Parameters.Add(new SqlParameter("@Role", login.Role));

                lCommand.ExecuteNonQuery();
            }
        }

        // Check if username exists
        public bool IsUserNameExists(string userName)
        {
            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                string query = "SELECT 1 FROM Users WHERE UserName = @UserName";
                IDbCommand command = _databaseHelper.CreateCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@UserName", userName));

                connection.Open();
                return command.ExecuteScalar() != null;
            }
        }

        // Get a single user by UserID
        public Users GetUser(int userID)
        {
            Users user = null;
            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                string query = "SELECT UserID, UserName, Age, PhoneNumber, TotalFine, Email, IsActive " +
                               "FROM Users WHERE UserID = @UserID";
                IDbCommand command = _databaseHelper.CreateCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@UserID", userID));

                connection.Open();
                IDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user = MapUser(reader); // Using the helper method
                }
            }

            if (user == null)
            {
                throw new Exception($"User with ID {userID} does not exist.");
            }
            return user;
        }

        // Edit user details
        public void EditUser(int userID, Users user)
        {
            Users existingUser = GetUser(userID);

            string updatedUserName = string.IsNullOrEmpty(user.UserName) ? existingUser.UserName : user.UserName;
            int updatedAge = user.Age == 0 ? existingUser.Age : user.Age;
            string updatedPhoneNumber = string.IsNullOrEmpty(user.PhoneNumber) ? existingUser.PhoneNumber : user.PhoneNumber;
            string updatedEmail = string.IsNullOrEmpty(user.Email) ? existingUser.Email : user.Email;
            bool updatedIsActive = user.IsActive != existingUser.IsActive ? user.IsActive : existingUser.IsActive;

            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                string query = "UPDATE Users SET UserName = @UserName, Age = @Age, PhoneNumber = @PhoneNumber, " +
                               "Email = @Email, IsActive = @IsActive WHERE UserID = @UserID";

                IDbCommand command = _databaseHelper.CreateCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@UserName", updatedUserName));
                command.Parameters.Add(new SqlParameter("@Age", updatedAge));
                command.Parameters.Add(new SqlParameter("@PhoneNumber", updatedPhoneNumber));
                command.Parameters.Add(new SqlParameter("@Email", updatedEmail));
                command.Parameters.Add(new SqlParameter("@IsActive", updatedIsActive));
                command.Parameters.Add(new SqlParameter("@UserID", userID));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Delete a user (soft delete)
        public void DeleteUser(int userID)
        {
            Users user = GetUser(userID); // Will throw if the user does not exist
            if (user != null)
            {
                if (user.TotalFine > 0)
                {
                    throw new Exception("User has outstanding fines.");
                }

                if (HasBorrowedBooks(userID))
                {
                    throw new Exception("User has borrowed books and cannot be deleted.");
                }

                if (!user.IsActive)
                {
                    throw new Exception($"User with ID {userID} is inactive.");
                }

                using (IDbConnection connection = _databaseHelper.CreateConnection())
                {
                    string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";
                    IDbCommand command = _databaseHelper.CreateCommand(query, connection);
                    command.Parameters.Add(new SqlParameter("@UserID", userID));

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool HasBorrowedBooks(int userID)
        {
            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                string query = "SELECT COUNT(1) FROM BorrowedBooks WHERE UserID = @UserID AND ReturnDate IS NULL";
                IDbCommand command = _databaseHelper.CreateCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@UserID", userID));

                connection.Open();
                int borrowedCount = Convert.ToInt32(command.ExecuteScalar());

                return borrowedCount > 0;
            }
        }

        public IEnumerable<Users> GetUser(string UserName)
        {
            Users user = null;
            List<Users> usersList = new List<Users>();

            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                string query = "SELECT UserID, UserName, Age, PhoneNumber, TotalFine, Email, IsActive " +
                               "FROM Users WHERE UserName LIKE @UserName";
                IDbCommand command = _databaseHelper.CreateCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@UserName", SqlDbType.NVarChar) { Value = "%" + UserName + "%" });


                connection.Open();
                IDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    user = MapUser(reader);// Using the helper method
                    usersList.Add(user);

                }
                reader.Close();
            }

            if (usersList.Count == 0)
            {
                throw new Exception($"No users found with a username containing '{UserName}'.");
            }

            return usersList;
        }
        public Users ReactivateUser(int userID)
        {
            Users user = null;
            using (IDbConnection connection = _databaseHelper.CreateConnection())
            {
                // Reactivate the user by updating the IsActive field
                string query = "UPDATE Users SET IsActive = 1 WHERE UserID = @UserID";
                using (IDbCommand command = _databaseHelper.CreateCommand(query, connection))
                {
                    // Correctly create the SqlParameter and add it to the command
                    var param = new SqlParameter("@UserID", SqlDbType.Int);
                    param.Value = userID;
                    command.Parameters.Add(param);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Fetch the updated user after reactivation
                        string fetchQuery = "SELECT * FROM Users WHERE UserID = @UserID";
                        using (IDbCommand fetchCommand = _databaseHelper.CreateCommand(fetchQuery, connection))
                        {
                            // Add parameter to fetch command
                            var fetchParam = new SqlParameter("@UserID", SqlDbType.Int);
                            fetchParam.Value = userID;
                            fetchCommand.Parameters.Add(fetchParam);

                            using (IDataReader reader = fetchCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    user = MapUser(reader);
                                }
                            }
                        }
                    }
                }
            }

            return user; // Return the user if found, otherwise null
        }



        // Helper method to map a reader to a Users object
        private Users MapUser(IDataReader reader)
        {
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
}
