using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.DTO
{
    /// <summary>
    /// Represents the request model for editing user details.
    /// </summary>
    public class EditUserRequest
    {
        /// <summary>
        /// Change username of the user.
        /// The username must be less than or equal to 20 characters.
        /// </summary>
        [StringLength(20, ErrorMessage = "UserName must be less than or equal to 20 characters.")]
        public string? UserName { get; set; }

        /// <summary>
        /// Change age of the user.
        /// The age must be between 6 and 100 years.
        /// </summary>
        [Range(6, 100, ErrorMessage = "Age must be greater than 5 and less than 100.")]
        public int? Age { get; set; }

        /// <summary>
        /// Change email address of the user.
        /// The email must be in a valid format.
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        /// <summary>
        /// Change phone number of the user.
        /// The phone number must start with 6, 7, 8, or 9 and contain 10 digits.
        /// </summary>
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "PhoneNumber must start with 6, 7, 8, or 9 and contain 10 digits and no special characters.")]
        public string? PhoneNumber { get; set; }
    }
}
