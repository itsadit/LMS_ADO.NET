using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DataAccessLayer
{
    public interface ILoginDAO
    {
        Login AuthenticateUser(string userName, string password);
        Login ValidateUserForPasswordReset(string userName, int userID);
        bool UpdatePassword(string userName, string newPassword);
    }
}
