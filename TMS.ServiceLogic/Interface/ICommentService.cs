using TMS.Contracts.Request;
using TMS.Contracts.Response;

namespace TMS.ServiceLogic.Interface
{
    public interface ICommentService
    {
        Task<CommentResponse> AddCommentAsync(int taskId, CreateCommentRequest request, int userId, string role);
        Task<List<CommentResponse>> GetCommentsByTaskAsync(int taskId, int userId, string role);
    }
}

