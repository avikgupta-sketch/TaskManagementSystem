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
           
            
                var callerRole = User.FindFirstValue(ClaimTypes.Role);
                if (callerRole != "SuperAdmin")
                    return StatusCode(403, new { message = "Only SuperAdmin can create Admin accounts." });
            
            var result = await _authService.RegisterAsync(request);
            return result switch
            {
                "Success" => Ok(new { message = "User registered Successfully." }),
                "AlreadyRegistered" => Conflict(new { message = "Email already registered. Login" }),
                _ => BadRequest()

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

       
    }
}
