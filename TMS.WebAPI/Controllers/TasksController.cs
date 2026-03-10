using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Contracts.Request;
using TMS.ServiceLogic.Interface;

namespace TMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService) => _taskService = taskService;

        [HttpPost("create")]
        [Authorize(Roles = "Admin")] // Only Admins can create
        public async Task<IActionResult> Create(CreateTaskRequest request)
        {
            // Extract Admin ID from JWT Claim
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _taskService.CreateTaskAsync(request, adminId);
            return Ok(result);
        }

    }
}
