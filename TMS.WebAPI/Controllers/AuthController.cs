using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.Model.Enums;
using TMS.ServiceLogic.Implementations;
using TMS.ServiceLogic.Interface;

namespace TMS.WebAPI.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return result switch
            {
                "Success" => Ok(new { message = "User registered Successfully." }),
                "AlreadyRegistered" => Conflict(new { message = "Email already registered. Login" }),
                _ => BadRequest(new {message ="Error! Try again later"})

            };
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Conflict(new { message = "Invalid Email or password" });
            return Ok(result);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _authService.DeleteUserAsync(id);

            return result switch
            {
                "Success" => Ok(new { message = "User successfully deleted." }),
                "User not found" => NotFound(new { message = result }),
                _ => BadRequest(new { message = result }) 
            };
        }
    }
}
