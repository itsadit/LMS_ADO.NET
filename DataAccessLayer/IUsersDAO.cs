using LibraryManagementSystem.Models;

public interface IUsersDAO
{
    public IEnumerable<Users> GetAllUsers();  // Retrieve all users
    public IEnumerable<Users> GetActiveUsers();
    public void AddUser(Users user,Login login);  // Add a new user
    public Users ReactivateUser(int userID); // Reactivating user
    public Users GetUser(int UserID);  // Retrieve a specific user by UserID
    public void EditUser(int UserID, Users user);  // Edit user details
    public void DeleteUser(int UserID);  // Delete a user by UserID
    public IEnumerable<Users> GetUser(string userName); // Retrieve a specific user by UserName
}
