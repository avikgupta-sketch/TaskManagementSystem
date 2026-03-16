using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.ServiceLogic.Implementations;
using TMS.ServiceLogic.Interface;
using static TMS.Model.Exceptions.Exceptions;

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
                return StatusCode(500, new { message = "An error occurred while fetching comments." });
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
                return StatusCode(500, new { message = "An error occurred while adding the comment." });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateComment(
         [FromRoute] int taskId,
         [FromBody] UpdateCommentRequest request)

        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
               
                var result = await _commentService.UpdateCommentAsync(taskId, request, userId);

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
                return StatusCode(500, new { message = "An error occurred while updating the comment." });
            }
        }

        
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(
            [FromRoute] int taskId,
            [FromBody] DeleteCommentRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                await _commentService.DeleteCommentAsync(taskId, request, userId);

                return Ok(new { message = "Comment deleted successfully." });
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
                return StatusCode(500, new { message = "An error occurred during comment deletion." });
            }
        }
    }
}
