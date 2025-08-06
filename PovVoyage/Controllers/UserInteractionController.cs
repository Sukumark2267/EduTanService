using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PovVoyage.Data.Entities;
using PovVoyage.Services.Interfaces;

namespace PovVoyage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInteractionController : ControllerBase
    {
        private readonly IUserInteractionService _userService; 
        public UserInteractionController(IUserInteractionService userService) 
        { 
            _userService = userService; 
        }
        [HttpPost("register")] 
        public async Task<IActionResult> Register(User user) 
        { 
            var registeredUser = await _userService.RegisterUserAsync(user); 
            return Ok(registeredUser); 
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _userService.LoginUserAsync(request.Username, request.Password);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] User user)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserProfileAsync(user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
