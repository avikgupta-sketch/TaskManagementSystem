using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.Model.Entities;
using TMS.ServiceLogic.Interface;


namespace TMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;


        public TasksController(ITaskService taskService, IMapper mapper) {
            _taskService = taskService;
            _mapper = mapper;

        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")] // Only Admins can create
        public async Task<IActionResult> Create(CreateTaskRequest request)
        {
            // Extract Admin ID from JWT Claim
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _taskService.CreateTaskAsync(request, adminId);

            if (result == null)
            {
                return BadRequest("The user you are trying to assign this task to does not exist.");
            }
            var response = _mapper.Map<TaskResponse>(result);
            return Ok(response);
        }


        [HttpPost("assign")]
        [Authorize(Roles = "Admin")] // Only Admins can assign
        public async Task<IActionResult> Assign(AssignTaskRequest request)
        {
            var success = await _taskService.AssignTaskAsync(request);
            if (!success) return BadRequest("Task or User not found.");

            return Ok(new { message = "Task assigned successfully" });
        }
        [HttpGet("view")]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var tasks = await _taskService.GetAllTasksAsync(userId, role);
            return Ok(tasks);
        }

        // GET /api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var task = await _taskService.GetTaskByIdAsync(id, userId, role);
            return Ok(task);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var task = await _taskService.UpdateTaskAsync(id, request, userId);
            return Ok(task);
        }

    }
}
