using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.ServiceLogic.Implementations;
using TMS.ServiceLogic.Interface;

namespace TMS.API.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId}/comments")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetComments(int taskId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var role = User.FindFirst(ClaimTypes.Role)!.Value;

                var comments = await _commentService.GetCommentsByTaskAsync(taskId, userId, role);
                return Ok(comments);
            }
            catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int taskId, [FromBody] CreateCommentRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var role = User.FindFirst(ClaimTypes.Role)!.Value;

                var comment = await _commentService.AddCommentAsync(taskId, request, userId, role);
                return Ok(comment);
            }
            catch (Exception ex) { 
                return BadRequest(new {message=ex.Message});
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateComment(
         [FromRoute] int taskId,
         [FromBody] UpdateCommentRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // We pass the taskId from the route and the whole DTO from the body
            var result = await _commentService.UpdateCommentAsync(taskId, request, userId);

            if (result == null)
            {
                return StatusCode(403, new
                {
                    message = "Access Denied: You are not the author."
                });
            }

            return Ok(result);
        }

        
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(
            [FromRoute] int taskId,
            [FromBody] DeleteCommentRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _commentService.DeleteCommentAsync(taskId, request, userId);

            return result switch
            {
                "Success" => Ok(new { message = "Comment deleted successfully (Soft Delete)." }),
                "Forbidden" => StatusCode(403, new { message = "Access Denied: You cannot delete this comment." }),
                "NotFound" => NotFound(new { message = "Comment not found or already deleted." }),
                _ => BadRequest()
            };
        }
    }
}
