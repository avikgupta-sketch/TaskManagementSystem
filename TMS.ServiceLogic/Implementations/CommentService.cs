using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Contracts.Request;
using TMS.Contracts.Response;
using TMS.Model.Data;
using TMS.Model.Entities;
using TMS.ServiceLogic.Interface;
using static TMS.Model.Exceptions.Exceptions;

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
                .FirstOrDefaultAsync(t => t.Id == taskId );

            if (task == null)
                throw new NotFoundException("Task not found.");


            if (role == "Admin" && task.CreatedByUserId != userId)
                throw new ForbiddenException("You can only comment on tasks you created.");

            if (role == "User" && task.AssignedToUserId != userId)
                throw new ForbiddenException("You can only comment on tasks assigned to you.");


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
                .FirstOrDefaultAsync(t => t.Id == taskId );



            // Task not found
            if (task == null)
                throw new NotFoundException("Task not found.");


            if (role == "User" && task.AssignedToUserId != userId)
                throw new ForbiddenException("You are not authorized to view comments on this task.");

            var comments = await _context.Comments
                .Include(c => c.Author)
                .Where(c => c.TaskItemId == taskId )
                .OrderBy(c => c.CreatedAt)          
                .ToListAsync();

            return _mapper.Map<List<CommentResponse>>(comments);
        }

        public async Task<CommentResponse?> UpdateCommentAsync(int taskId, UpdateCommentRequest request, int userId)
        {

            var task = await _context.TaskItems
              .FirstOrDefaultAsync(t => t.Id == taskId );

            // Task not found
            if (task == null)
                throw new NotFoundException("Task not found.");
           
            var comment = await _context.Comments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == request.CommentId );

            if(comment == null || comment.TaskItemId != taskId)
            {
                throw new NotFoundException("Comment not found on this task.");
            }

            if (comment.UserId != userId)
            {
                throw new ForbiddenException("Access Denied: You can only edit your own comments.");
            }

            comment.Message = request.Message;

            await _context.SaveChangesAsync();

            return _mapper.Map<CommentResponse>(comment);
        }

        public async Task<bool> DeleteCommentAsync(int taskId, DeleteCommentRequest request, int userId)
        {
            var task = await _context.TaskItems
              .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                throw new NotFoundException("Task not found.");

            
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == request.CommentId );

            if (comment == null) throw new NotFoundException("Comment not found.");
           
            if (comment.TaskItemId != taskId) throw new ForbiddenException("Access Denied: You cannot delete this comment.");
          
            if (comment.UserId != userId) throw new ForbiddenException("Access Denied: You cannot delete this comment.");


            comment.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
