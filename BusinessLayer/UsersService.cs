using LibraryManagementSystem.DataAccessLayer;
using LibraryManagementSystem.Models;
using System.Collections.Generic;

namespace LibraryManagementSystem.BusinessLayer
{
    public class UsersService : IUsersService
    {
        private readonly UsersDAO _usersDAO;

        public UsersService(UsersDAO usersDAO)
        {
            _usersDAO = usersDAO;
        }

        // Fetch all users
        public List<Users> GetAllUsers()
        {
            return (List<Users>)_usersDAO.GetAllUsers();
        }

        public List<Users> GetActiveUsers()
        {
            return(List<Users>) _usersDAO.GetActiveUsers();
        }

        // Fetch a user by ID
        public Users GetUserById(int userID)
        {
            return _usersDAO.GetUser(userID);
        }

        public List<Users> GetUserByName(string userName)
        {
            return(List<Users>) _usersDAO.GetUser(userName);
        }
        // Add a user with uniqueness check for username
        public void AddUserWithCheck(Users user, Login login)
        {
            bool existingUser = _usersDAO.IsUserNameExists(user.UserName);
            if (!existingUser)
            {
                _usersDAO.AddUser(user, login);
            }
            else
            {
                throw new System.Exception("Username already exists.");
            }
        }
        public Users ReactivateUser(int userID)
        {
            return _usersDAO.ReactivateUser(userID); // This will return the reactivated user
        }



        // Delete user by ID
        public void DeleteUser(int userID)
        {
            // First, fetch the user data to be archived
            _usersDAO.DeleteUser(userID);
        }
        public void EditUser(int userID, Users user)
        {
            _usersDAO.EditUser(userID, user);
        }
    }
}
