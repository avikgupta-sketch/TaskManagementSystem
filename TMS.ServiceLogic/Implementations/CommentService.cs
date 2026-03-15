using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.Model.Entities;
using AutoMapper;
using TMS.Model.Data;
using TMS.ServiceLogic.Interface;

namespace TMS.ServiceLogic.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CommentService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
        public async Task<CommentResponse> AddCommentAsync(int taskId, CreateCommentRequest request, int userId, string role)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);

            if (task == null)
                throw new Exception("Task not found");

           
            if (role == "Admin" && task.CreatedByUserId != userId)
                throw new Exception("You can only comment on tasks you created");

            if (role == "User" && task.AssignedToUserId != userId)
                throw new Exception("You can only comment on tasks assigned to you");

            
            var comment = _mapper.Map<Comment>(request);  
            comment.UserId = userId;
            comment.TaskItemId = taskId;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var saved = await _context.Comments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            return _mapper.Map<CommentResponse>(saved);
        }
        public async Task<List<CommentResponse>> GetCommentsByTaskAsync(int taskId, int userId, string role)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);



            // Task not found
            if (task == null)
                throw new Exception("Task not found");

            
            if (role == "User" && task.AssignedToUserId != userId)
                throw new Exception("You are not authorized to view comments on this task");

            var comments = await _context.Comments
                .Include(c => c.Author)
                .Where(c => c.TaskItemId == taskId && !c.IsDeleted)
                .OrderBy(c => c.CreatedAt)          
                .ToListAsync();

            return _mapper.Map<List<CommentResponse>>(comments);
        }

        public async Task<CommentResponse?> UpdateCommentAsync(int taskId, UpdateCommentRequest request, int userId)
        {

            var task = await _context.TaskItems
              .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);

            // Task not found
            if (task == null)
                throw new Exception("Task not found");


            // Fetch comment including the Author
            var comment = await _context.Comments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == request.CommentId && !c.IsDeleted);

            if(comment == null || comment.TaskItemId != taskId)
            {
                throw new Exception("Comment not found");
            }

            if (comment.UserId != userId)
            {
                return null;
            }

            //  Update the message
            comment.Message = request.Message;

            await _context.SaveChangesAsync();

            return _mapper.Map<CommentResponse>(comment);
        }

        public async Task<string> DeleteCommentAsync(int taskId, DeleteCommentRequest request, int userId)
        {
            //  Fetch the comment only if not already soft deleted
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == request.CommentId && !c.IsDeleted);

            if (comment == null) return "NotFound";

            //  Logic Check: Does this comment belong to the taskId in the URL
            if (comment.TaskItemId != taskId) return "Forbidden";

            //  Security Check: Did this user create the comment?
            if (comment.UserId != userId) return "Forbidden";

            
            comment.IsDeleted = true;

            await _context.SaveChangesAsync();

            return "Success";
        }
    }
}
