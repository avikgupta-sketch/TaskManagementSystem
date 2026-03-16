using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.Model.Entities;
using TMS.ServiceLogic.Interface;
using static TMS.Model.Exceptions.Exceptions;


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
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {         
            try
            {
                var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _taskService.CreateTaskAsync(request, adminId);
                var response = _mapper.Map<TaskResponse>(result);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
            catch (DuplicateTaskException ex)
            {
                return Conflict(new { message = ex.Message }); // 409
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex)
            {             
                return StatusCode(500, new { message = "An internal server error occurred." });
            }

        }


        [HttpPost("assign")]
        [Authorize(Roles = "Admin")] // Only Admins can assign
        public async Task<IActionResult> Assign([FromBody] AssignTaskRequest request)
        {
            try
            {
                var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                // 2. Call Service (This might throw custom exceptions)
                await _taskService.AssignTaskAsync(request, adminId);

                // 3. If we reach here, no exception was thrown
                return Ok(new { message = "Task assigned successfully." });
            }
            catch (NotFoundException ex)
            {
                // Handled as 404
                return NotFound(new { message = ex.Message });
            }
            catch (DuplicateTaskException ex)
            {
                // Handled as 409
                return Conflict(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                // Handled as 400
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Final fallback for unexpected errors (500)
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }

        }
        [HttpGet("view")]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role)!;

            var tasks = await _taskService.GetAllTasksAsync(userId, role);
            return Ok(tasks);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var role = User.FindFirst(ClaimTypes.Role)!.Value;

                var task = await _taskService.GetTaskByIdAsync(id, userId, role);
                return Ok(task);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the task." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var task = await _taskService.UpdateTaskAsync(id, request, userId);
                return Ok(task);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message }); // 403
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400
            }
            catch (Exception ex)
            {               
                return StatusCode(500, new { message = "An internal error occurred during update." });
            }
        }

        [HttpPatch("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTaskStatusRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var role = User.FindFirstValue(ClaimTypes.Role)!;

                var result = await _taskService.UpdateTaskStatusAsync(request, userId, role);

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message }); // 403
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task status." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
            
        {


            try
            {
                var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                await _taskService.DeleteTaskAsync(id, adminId);

                return Ok(new { message = "Task deleted successfully." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (ValidationException ex)
            {                
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during deletion." });
            }
        }

    }
}
