using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.Model.Enums;
using TMS.ServiceLogic.Implementations;
using TMS.ServiceLogic.Interface;
using static TMS.Model.Exceptions.Exceptions;

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
            try
            {
                await _authService.RegisterAsync(request);

                return Ok(new { message = "Registered successfully." });
            }
            catch (DuplicateTaskException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred during registration. Please try again later." });
            }
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
            try
            {               
                await _authService.DeleteUserAsync(id);

                return Ok(new { message = "User successfully deleted." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the user." });
            }
        }
    }
}
