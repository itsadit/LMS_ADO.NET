using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Models.Enum;

namespace LibraryManagementSystem.Models.DTO
{
    /// <summary>
    /// Represents the request model for creating a new user.
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// Username of the user.
        /// The username is required and must be less than or equal to 20 characters.
        /// </summary>
        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(20, ErrorMessage = "UserName must be less than or equal to 20 characters.")]
        public string UserName { get; set; }

        /// <summary>
        /// Age of the user.
        /// The age must be between 6 and 100 years.
        /// </summary>
        [Range(6, 100, ErrorMessage = "Age must be between 6 and 100")]
        public int Age { get; set; }

        /// <summary>
        /// Phone number of the user.
        /// The phone number is required, must start with 6, 7, 8, or 9, and contain 10 digits.
        /// </summary>
        [Required(ErrorMessage = "PhoneNumber is required.")]
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "PhoneNumber must start with 6, 7, 8, or 9 and contain 10 digits and no special characters.")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email address of the user.
        /// The email is required and must be in a valid email format.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        /// <summary>
        /// Password of the user.
        /// The password is required and must be at least 8 characters long.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password length should be greater than or equal to 8 characters.")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmation password for the user.
        /// The confirmation password is required and must match the original password.
        /// </summary>
        [Required(ErrorMessage = "ConfirmPassword is required.")]
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Role of the user.
        /// The role is required and can only be "Admin" or "User".
        /// </summary>
        [Required(ErrorMessage = "Role is required.")]
        public UserRole Role { get; set; } // This will now only accept Admin or User
    }
}
