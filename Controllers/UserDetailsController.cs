using LibraryManagementSystem.BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        public readonly IUsersService _usersService;

        public UserDetailsController(IUsersService usersService)
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
    }
}
