namespace LibraryAPI.DataAccess.Models
{
    public class User
    {
        public int UserID { get; set; } // Primary Key, auto-incremented

        public string UserName { get; set; } // Unique username

        public int Age { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public decimal TotalFine { get; set; } = 0;  // Default value 0

        public bool IsActive { get; set; } = true;  // Default value true (1)
    }
}
