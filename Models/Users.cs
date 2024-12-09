public class Users
{
    public int UserID { get; set; } //Primary Key Auto-Generated
    public string UserName { get; set; } // Unique Key
    public int Age { get; set; } // Age should be greater than 5
    public string? Email { get; set; }  // Nullable, can be left out in the request
    public string? PhoneNumber { get; set; }  // Nullable, can be left out in the request
    public decimal TotalFine { get; set; } //Fine amount to be paid by the user
    public bool IsActive { get; set; } //checks whether user is active or not
}
