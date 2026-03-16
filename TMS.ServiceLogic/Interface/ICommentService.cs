using TMS.Contracts.Request;
using TMS.Contracts.Response;

namespace TMS.ServiceLogic.Interface
{
    public interface ICommentService
    {
        Task<CommentResponse> AddCommentAsync(int taskId, CreateCommentRequest request, int userId, string role);

        Task<List<CommentResponse>> GetCommentsByTaskAsync(int taskId, int userId, string role);

        Task<CommentResponse?> UpdateCommentAsync(int taskId, UpdateCommentRequest request, int userId);

        Task<bool> DeleteCommentAsync(int taskId, DeleteCommentRequest request, int userId);
    }
}

