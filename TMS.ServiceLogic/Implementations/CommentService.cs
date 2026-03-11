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

namespace TMS.ServiceLogic.Implementation
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
                .Where(c => c.TaskItemId == taskId)
                .OrderBy(c => c.CreatedAt)          
                .ToListAsync();

            return _mapper.Map<List<CommentResponse>>(comments);
        }

    }
}
