using LibraryManagementSystem.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using LibraryManagementSystem.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace LibraryManagementSystem.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LoginDAO _loginDAO;
        private readonly IConfiguration _configuration;

        // Static variable to store logged-in user's details
        private static string? _currentlyLoggedInUser = null;

        /// <summary>
        /// Initializes a new instance of the LoginController.
        /// </summary>
        /// <param name="configuration">The configuration for the application.</param>
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _loginDAO = new LoginDAO(configuration);
        }

        /// <summary>
        /// Logs in a user by validating the provided username and password.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A response indicating the result of the login attempt.</returns>
        [HttpGet("login")]
        public IActionResult Login([FromQuery] string userName, [FromQuery] string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username and password are required.");
            }

            // Check if another user is already logged in
            if (_currentlyLoggedInUser != null && _currentlyLoggedInUser != userName)
            {
                return Conflict(new { Message = "Already another user is logged in." });
            }

            // Synchronously authenticate the user
            var login = _loginDAO.AuthenticateUser(userName, password);

            if (login != null)
            {
                // Set the logged-in user
                _currentlyLoggedInUser = login.UserName;

                var token = GenerateJwtToken(login); // Generate JWT token that includes the role

                // Create a success message along with the token
                return Ok(new
                {
                    Message = "User logged in successfully",
                    Token = token
                });
            }
            else
            {
                // If login fails, return an error response
                return Unauthorized(new { Message = "Invalid credentials" });
            }
        }

        /// <summary>
        /// Logs out the current user by clearing the logged-in user.
        /// </summary>
        /// <returns>A response indicating the result of the logout attempt.</returns>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (_currentlyLoggedInUser == null)
            {
                return BadRequest(new { Message = "No user is currently logged in." });
            }

            // Clear the currently logged-in user
            _currentlyLoggedInUser = null;
            return Ok(new { Message = "User logged out successfully." });
        }

        /// <summary>
        /// Allows the user to reset their password by verifying username and user ID.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="newPassword">The new password to be set.</param>
        /// <returns>A response indicating the result of the password reset operation.</returns>
        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword([FromQuery] string userName, [FromQuery] int userID, [FromQuery] string newPassword)
        {
            // Check if both username and userID match in the database
            var login = _loginDAO.ValidateUserForPasswordReset(userName, userID);

            if (login != null)
            {
                // User is valid, update the password in the database
                var result = _loginDAO.UpdatePassword(userName, newPassword);

                if (result)
                {
                    return Ok(new { Message = "Password updated successfully." });
                }
                return StatusCode(500, new { Message = "Failed to update password." });
            }

            return BadRequest(new { Message = "Invalid username or userID." });
        }

        /// <summary>
        /// Generates a JWT token for the authenticated user.
        /// </summary>
        /// <param name="login">The login object containing user details.</param>
        /// <returns>A JWT token string.</returns>
        private string GenerateJwtToken(Login login)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, login.UserName),
                new Claim(ClaimTypes.Role, login.Role), // Include the role in the claims
                new Claim(ClaimTypes.NameIdentifier, login.UserID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Resets the password for a user by validating their username and current password.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="currentPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password to be set.</param>
        /// <returns>A response indicating the result of the password reset operation.</returns>
        [HttpGet("reset-password")]
        public IActionResult ResetPassword([FromQuery] string userName, [FromQuery] string currentPassword, [FromQuery] string newPassword)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Username, current password, and new password are required.");
            }

            // Validate user and current password
            var login = _loginDAO.AuthenticateUser(userName, currentPassword);

            if (login != null)
            {
                // User is valid, update the password in the database
                var result = _loginDAO.UpdatePassword(userName, newPassword);

                if (result)
                {
                    return Ok(new { Message = "Password updated successfully." });
                }
                return StatusCode(500, new { Message = "Failed to update password." });
            }

            return Unauthorized(new { Message = "Invalid credentials." });
        }
    }

    /// <summary>
    /// Request model for forgot password functionality.
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// User's username for the password reset.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User's user ID for the password reset.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Set the new password.
        /// </summary>
        public string NewPassword { get; set; }
    }
}
