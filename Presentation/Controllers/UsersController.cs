using LibraryManagementSystem.Models;
using LibraryManagementSystem.BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementSystem.Models.DTO;

namespace LibraryManagementSystem.Presentation.Controllers
{
    public class UsersController : ControllerBase
    {
        public readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;  // Constructor injection
        }

        /// <summary>
        /// Retrieves the logged-in user's details.
        /// </summary>
        /// <returns>Returns the user data if found, or an error message if not.</returns>
        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetMyDetails")]
        public IActionResult GetUserDetails()
        {
            try
            {
                var userName = User.Identity.Name;  // Get the logged-in user's username

                if (string.IsNullOrEmpty(userName))
                {
                    return BadRequest("User is not logged in.");
                }

                var user = _usersService.GetUserByName(userName);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(user);  // Return the user data
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers a new user with their related details (e.g., Username, Email, Age, Phone number).
        /// </summary>
        /// <param name="request">The user details for registration.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterUser")]
        public IActionResult CreateUser([FromForm] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create the new user object
            Users user = new Users
            {
                UserName = request.UserName,
                Age = request.Age,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };

            // Create the login object with the username and password
            Login login = new Login
            {
                UserName = user.UserName,
                Password = request.Password,
                Role = request.Role.ToString()
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

        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        /// <returns>A list of all users.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("AllUsers")]
        public IActionResult GetUsers()
        {
            var usersList = _usersService.GetAllUsers();
            return Ok(usersList);
        }

        /// <summary>
        /// Retrieves all active users in the system.
        /// </summary>
        /// <returns>A list of all active users.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("AllActiveUsers")]
        public IActionResult GetActiveUsers()
        {
            var usersList = _usersService.GetActiveUsers();
            return Ok(usersList);
        }

        /// <summary>
        /// Retrieves a user by their unique ID.
        /// </summary>
        /// <param name="userID">The ID of the user to retrieve.</param>
        /// <returns>The user data if found, or an error message if not.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("SearchUsersBy/UserID")]
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

        /// <summary>
        /// Retrieves a user by their unique username.
        /// </summary>
        /// <param name="userName">The username of the user to retrieve.</param>
        /// <returns>The user data if found, or an error message if not.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("SearchUsersBy/UserName")]
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

        /// <summary>
        /// Reactivates a user by their unique ID.
        /// </summary>
        /// <param name="userID">The ID of the user to reactivate.</param>
        /// <returns>The reactivated user data or an error message if not found.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("ReactivateUser")]
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
                return Ok(new { message = "User reactivated successfully.", reactivatedUser }); // Return the reactivated user with the UserID
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Edits the details of an existing user.
        /// </summary>
        /// <param name="userId">The ID of the user to edit.</param>
        /// <param name="request">The details to update the user with.</param>
        /// <returns>A response indicating the result of the update operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("EditUser")]
        public IActionResult EditUser(int userId, [FromForm] EditUserRequest request)
        {
            try
            {
                // Fetch the existing user from the database
                Users existingUser = _usersService.GetUserById(userId);


                // Track changes made
                var changes = new List<string>();

                // Check if any valid fields are provided
                if (request.UserName != null && request.UserName != existingUser.UserName)
                {
                    existingUser.UserName = request.UserName; // Update username if provided
                    changes.Add($"UserName changed to: {request.UserName}");
                }

                if (request.Age.HasValue && request.Age.Value != existingUser.Age)
                {
                    existingUser.Age = request.Age.Value; // Update age if provided
                    changes.Add($"Age changed to: {request.Age.Value}");
                }

                if (request.Email != null && request.Email != existingUser.Email)
                {
                    existingUser.Email = request.Email; // Update email if provided
                    changes.Add($"Email changed to: {request.Email}");
                }

                if (request.PhoneNumber != null && request.PhoneNumber != existingUser.PhoneNumber)
                {
                    existingUser.PhoneNumber = request.PhoneNumber; // Update phone number if provided
                    changes.Add($"PhoneNumber changed to: {request.PhoneNumber}");
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

        /// <summary>
        /// Deletes (deactivates) a user by their unique ID.
        /// </summary>
        /// <param name="userID">The ID of the user to delete.</param>
        /// <returns>No content if the operation is successful, or an error message if not.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("DeleteUser")]
        public IActionResult DeleteUser(int userID)
        {
            try
            {
                _usersService.DeleteUser(userID); // Call the service method to deactivate the user
                return Ok(new { message = "User deactivated successfully."});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }
    }
}
