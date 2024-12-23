using LibraryManagementSystem.Models;
using System.Collections.Generic;

namespace LibraryManagementSystem.BusinessLayer
{
    public interface IUsersService
    {
        List<Users> GetAllUsers();  // Fetch all users
        List<Users> GetActiveUsers();
        Users GetUserById(int userID);  // Fetch a single user by ID
        void AddUserWithCheck(Users user, Login login);  // Add user with uniqueness check
        public Users ReactivateUser(int userID);
        void DeleteUser(int userID);  // Delete user by ID
        void EditUser(int userID, Users user);//Update User by ID and user details
        List<Users> GetUserByName(string userName); // Fetch a single user by name
    }
}
