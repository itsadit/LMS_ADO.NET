using LibraryManagementSystem.Models;
using LibraryManagementSystem.BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using LibraryManagementSystem.DataAccessLayer;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;  // Constructor injection
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet("Get My Details")]
        public IActionResult GetUserDetails()
        {
            try
            {
                var userName = User.Identity.Name;  // Get the logged-in user's username

                if (string.IsNullOrEmpty(userName))
                {
                    return BadRequest("User is not logged in.");
                }

                // If the user is Admin, allow access to all users
                if (User.IsInRole("Admin"))
                {
                    var usersList = _usersService.GetAllUsers();
                    return Ok(usersList);
                }
                else
                {
                    // If the user is a regular user, fetch only their details
                    var user = _usersService.GetUserByName(userName);
                    if (user == null)
                    {
                        return NotFound("User not found.");
                    }
                    return Ok(user);  // Return the user data
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        // Action for creating a new user (POST api/users)
        [HttpPost("Register User")]
        public IActionResult CreateUser(string userName, int age, string phoneNumber, string email,
            string password, string confirmPassword, string role, decimal totalFine = 0)
        {
            // Validate username: first 3 letters should not be numbers
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("UserName is required.");
            }
            if (age == 0) // default value check for int
            {
                return BadRequest("Age is required.");
            }
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return BadRequest("PhoneNumber is required.");
            }
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Password is required.");
            }
            if (userName.Length > 20)
            {
                return BadRequest("UserName must be less than or equal to 20 characters.");
            }
            if (Regex.IsMatch(userName.Substring(0, 3), @"^\d{3}$"))
            {
                return BadRequest("First 3 characters of UserName cannot be digits.");
            }

            // Validate age: must be greater than 5 and not default (0)
            if (age <= 5 || age > 100)
            {
                return BadRequest("Age must be greater than 5 and less than 100");
            }


            // Validate phoneNumber: must not be empty and must start with 6, 7, 8, or 9 and contain 10 digits

            if (!Regex.IsMatch(phoneNumber, @"^[6789]\d{9}$"))
            {
                return BadRequest("PhoneNumber must start with 6, 7, 8, or 9 and contain 10 digits.");
            }

            // Validate email: must not be empty and match a valid email format

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                return BadRequest("Invalid email format.");
            }

            // Validate totalFine: must not be negative
            if (totalFine < 0)
            {
                return BadRequest("TotalFine cannot be negative.");
            }

            // Normalize role input by trimming any whitespace and capitalizing
            role = role?.Trim();

            // Convert to proper case ('Admin' or 'User')
            if (role.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                role = "Admin"; // Convert to "Admin" if "admin" is provided
            }
            else if (role.Equals("user", StringComparison.OrdinalIgnoreCase))
            {
                role = "User"; // Convert to "User" if "user" is provided
            }

            // Validate Role: case-sensitive check for "Admin" or "User"
            if (role != "Admin" && role != "User")
            {
                return BadRequest("Invalid role. Please choose either 'Admin' or 'User'.");
            }

            // Validate password: length should be greater than or equal to 8 characters
            if (password.Length < 8)
            {
                return BadRequest("Password length should be greater than or equal to 8 characters.");
            }

            // Validate password confirmation: must match password
            if (password != confirmPassword)
            {
                return BadRequest("Password doesn't match.");
            }


            // Create the new user object
            Users user = new Users
            {
                UserName = userName,
                Age = age,
                PhoneNumber = phoneNumber,
                Email = email,
                TotalFine = totalFine,
            };

            // Create the login object with the username and password
            Login login = new Login
            {
                UserName = user.UserName,
                Password = password,
                Role = role
            };

            try
            {
                // Add user with username uniqueness check and create the user and login in the database
                _usersService.AddUserWithCheck(user, login);

                // Return the new user with the auto-generated UserID
                return Ok(new { message = "User registered successfully.", userId = user.UserID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles ="Admin")]
        // Action for retrieving all users (GET api/users)
        [HttpGet("All Users")]
        public IActionResult GetUsers()
        {
            var usersList = _usersService.GetAllUsers();
            return Ok(usersList);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("All active Users")]
        public IActionResult GetActiveUsers()
        {
            var usersList = _usersService.GetActiveUsers();
            return Ok(usersList);
        }

        [Authorize(Roles = "Admin")]
        // Action for retrieving a single user by ID (GET api/users/{userID})
        [HttpGet("id/Search users by {userID}")]
        public IActionResult GetUserById(int userID)
        {
            try
            {
                var user = _usersService.GetUserById(userID);
                return Ok(user);  // Return the user data
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);  // Return error message as 404
            }
        }
        //Action for retrieving a single user by UserName

        [Authorize(Roles = "Admin")]
        [HttpGet("name/Search users by {userName}")]
        public IActionResult GetUserByName(string userName)
        {
            try
            {
                var user = _usersService.GetUserByName(userName);
                return Ok(user);  // Return the user data
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);  // Return error message as 404
            }
        }
        

        [Authorize(Roles = "Admin")]
        [HttpPost("Reactivate User")]
        public IActionResult ReactivateUser(int userID)
        {
            try
            {
                var user = _usersService.GetUserById(userID);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (user.IsActive)  // Assuming 'IsActive' is a property indicating the user's active status
                {
                    return BadRequest("User is already active.");
                }

                
                var reactivatedUser = _usersService.ReactivateUser(userID);
                return Ok(reactivatedUser); // Return the reactivated user with the UserID
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("Edit User")]
        public IActionResult EditUser(int userId, string userName = null,
                    int? age = null, string email = null, string phoneNumber = null)
        {
            try
            {
                // Fetch the existing user from the database
                Users existingUser = _usersService.GetUserById(userId);
                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found." });
                }
                if (!existingUser.IsActive)
                {
                    return BadRequest("User is not active.");
                }

                // Track changes made
                var changes = new List<string>();

                // Only update fields provided in the request
                if (!string.IsNullOrEmpty(userName))
                {
                    if (userName.Length > 20)
                    {
                        return BadRequest("UserName must be less than or equal to 20 characters.");
                    }
                    if (Regex.IsMatch(userName.Substring(0, 3), @"^\d{3}$"))
                    {
                        return BadRequest("First 3 characters of UserName cannot be digits.");
                    }
                    existingUser.UserName = userName; // Update username if provided
                    changes.Add($"UserName changed to: {userName}");
                }

                if (age.HasValue)
                {
                    // Validate age: must be greater than 5
                    if (age.Value <= 5 || age.Value > 100)
                    {
                        return BadRequest("Age must be greater than 5 and less than 100");
                    }
                    existingUser.Age = age.Value;  // Update age if provided
                    changes.Add($"Age changed to: {age.Value}");
                }

                if (!string.IsNullOrEmpty(email))
                {
                    // Validate email: must not be empty and match a valid email format
                    if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                    {
                        return BadRequest("Invalid email format.");
                    }
                    existingUser.Email = email;  // Update email if provided
                    changes.Add($"Email changed to: {email}");
                }

                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    // Validate phoneNumber: must start with 6, 7, 8, or 9 and contain 10 digits
                    if (!Regex.IsMatch(phoneNumber, @"^[6789]\d{9}$"))
                    {
                        return BadRequest("PhoneNumber must start with 6, 7, 8, or 9 and contain 10 digits.");
                    }
                    existingUser.PhoneNumber = phoneNumber;  // Update phone number if provided
                    changes.Add($"PhoneNumber changed to: {phoneNumber}");
                }

                

                // Call the service to update the user details in the database
                _usersService.EditUser(userId, existingUser);

                // Return only the changes made to the user
                if (changes.Count > 0)
                {
                    return Ok(new { message = "User updated successfully.", changes });
                }
                else
                {
                    return BadRequest("No valid changes were provided.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        // Action for deleting a user (DELETE api/users/{userID})
        [HttpDelete("Delete User {userID}")]
        public IActionResult DeleteUser(int userID)
        {
            try
            {
                _usersService.DeleteUser(userID); // Call the service method to deactivate the user
                return NoContent(); // Return 204 No Content on successful deactivation
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        } 
    }
}
