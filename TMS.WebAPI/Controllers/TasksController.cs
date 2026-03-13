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
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
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
        public async Task<IActionResult> Assign([FromBody] AssignTaskRequest request)
        {
            var result=await _taskService.AssignTaskAsync(request);
            return result switch
            {
                "Success" => Ok(new { message = "Task assigned successfully." }),
                "AlreadyAssigned" => Conflict(new { message = "Task is already assigned to this user." }),
                "NotFound" => NotFound(new { message = "Task or User not found." }),
                _ => BadRequest()

            };
           
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

        [HttpPatch("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTaskStatusRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            var result = await _taskService.UpdateTaskStatusAsync(request, userId, role);


            if (result == null)
                return StatusCode(403, new
                {
                    message = "Access Denied: You can only update status for tasks you created (Admin) or tasks assigned to you (User)."
                });


            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
            
        {
            // Extract the ID of the Admin currently logged in
            
            
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _taskService.DeleteTaskAsync(id, adminId);

            return result switch
            {
                "Success" => Ok(new { message = "Task deleted successfully." }),
                "Forbidden" => StatusCode(403, new { message = "Access Denied: You can only delete tasks created by you." }),
                "NotFound" => NotFound(new { message = "Task not found." }),
                "InProgress" => StatusCode(409, new { message = "Task cannot be deleted because it is currently in progress." }),
                "Completed" => StatusCode(409, new { message = "Task cannot be deleted because it is already completed." }),
                _ => BadRequest()
            };
        }

    }
}
