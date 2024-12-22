

namespace LibraryManagementSystem.Models
{
    public class Login
    {
        
        
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public virtual Users User { get; set; }  
    }
}
