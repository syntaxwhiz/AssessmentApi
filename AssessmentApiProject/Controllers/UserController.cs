using AssessmentApiProject.Models;
using AssessmentApiProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssessmentApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

         [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest("Invalid user data.");
        }

        string result = await _userService.AddUser(user);
        if (result == "User added successfully.")
        {
            return Ok(result);
        }
        else
        {
            return Conflict(result); // 409 Conflict for duplicate name
        }
    }

    [HttpPut("{name}")]
    public async Task<IActionResult> UpdateUser(string name, [FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest("Invalid user data.");
        }

        string result = await _userService.UpdateUser(name, user);
        if (result == "User updated successfully.")
        {
            return Ok(result);
        }
        else if (result == "User not found.")
        {
            return NotFound(result); // 404 Not Found if the user with given name doesn't exist
        }
        else
        {
            return BadRequest(result);
        }
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteUser(string name)
    {
        string result = await _userService.DeleteUser(name);
        if (result == "User deleted successfully.")
        {
            return Ok(result);
        }
        else if (result == "User not found.")
        {
            return NotFound(result); // 404 Not Found if the user with given name doesn't exist
        }
        else
        {
            return BadRequest(result);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        List<User> useres = await _userService.GetAllUsers();
        return Ok(useres);
    }
    }
}
