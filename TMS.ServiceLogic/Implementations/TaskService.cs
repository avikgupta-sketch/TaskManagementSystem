using AutoMapper;
using Azure.Core;
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
using TMS.Model.Enums;
using TMS.ServiceLogic.Interface;

namespace TMS.ServiceLogic.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TaskService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TaskItem?> CreateTaskAsync(CreateTaskRequest request, int adminId)
        {
            if (request.AssignedToUserId.HasValue)
            {
                var assignedUser = await _context.Users
           .FirstOrDefaultAsync(u => u.Id == request.AssignedToUserId.Value);
                

                if (assignedUser==null)
                    throw new Exception("Assigned user not found.");

                if (assignedUser.Role != UserRole.User)
                    throw new Exception("Tasks can only be assigned to user.");

            }

            var newTask = _mapper.Map<TaskItem>(request);
            newTask.CreatedByUserId = adminId;
            newTask.Status = TMS.Model.Enums.TaskStatus.Pending;

            _context.TaskItems.Add(newTask);
            await _context.SaveChangesAsync();

            
            var savedTask = await _context.TaskItems
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == newTask.Id);

            return savedTask;
        }

        public async Task<string> AssignTaskAsync(AssignTaskRequest request, int adminId)
        {

            var task = await _context.TaskItems.FindAsync(request.TaskId);
            var AssignedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            

            if (task == null ) return "TaskNotFound";
            if (AssignedUser==null) return "UserNotFound";
            if (AssignedUser.Role != UserRole.User) return "CanOnlyAssignUser";
            
            if (task.CreatedByUserId != adminId) return "Forbidden";
            if (task.AssignedToUserId != null) return "AlreadyAssigned";


            task.AssignedToUserId = request.UserId;
            await _context.SaveChangesAsync();
            return "Success";
        }
        public async Task<List<TaskResponse>> GetAllTasksAsync(int userId, string role)
        {
            IQueryable<TaskItem> query = _context.TaskItems
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo);
                
                

            // Admin sees all, User sees only assigned
            if (role == "User")
                query = query.Where(t => t.AssignedToUserId == userId);

            var tasks = await query.ToListAsync();
            return _mapper.Map<List<TaskResponse>>(tasks);
        }

        public async Task<TaskResponse> GetTaskByIdAsync(int taskId, int userId, string role)
        {
            var task = await _context.TaskItems
                 .Include(t => t.CreatedBy)
                 .Include(t => t.AssignedTo)
                 .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted); 

            if (task == null)
                throw new Exception("Task not found");

            // User can only see their assigned task
            if (role == "User" && task.AssignedToUserId != userId)
                throw new Exception("You are not authorized to view this task");

            return _mapper.Map<TaskResponse>(task);
        }
        public async Task<TaskResponse> UpdateTaskAsync(int taskId, UpdateTaskRequest request, int userId)
        {
            var task = await _context.TaskItems
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == taskId );

            // Task not found
            if (task == null)
                throw new Exception("Task not found");

            // Ownership check — only creator admin can update 
            if (task.CreatedByUserId != userId)
                throw new Exception("You can only update tasks you created");

            // Check if new AssignedToUserId exists
            if (request.AssignedToUserId.HasValue)
            {
                var AssignedUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == request.AssignedToUserId.Value);
                if (AssignedUser==null)
                    throw new Exception("Assigned user not found");
                if (AssignedUser.Role != UserRole.User)
                    throw new Exception("Task can only be assigned to user.");
            }

            // Map only updated fields
            _mapper.Map(request, task);
            await _context.SaveChangesAsync();

            // Reload to get updated navigation properties
            var updatedTask = await _context.TaskItems
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            return _mapper.Map<TaskResponse>(updatedTask);
        }

        public async Task<TaskResponse?> UpdateTaskStatusAsync(UpdateTaskStatusRequest request, int userId, string userRole)
        {

            var task = await _context.TaskItems
              .Include(t => t.CreatedBy)
              .Include(t => t.AssignedTo)
              .FirstOrDefaultAsync(t => t.Id == request.TaskId );

            

            if (task == null) return null;

            // Authorization logic stays the same but uses the DTO/Context
            if (userRole == "Admin")
            {
                if (task.CreatedByUserId != userId) 
                    throw new Exception("The task is not created by you."); 
            }
            else
            {
                if (task.AssignedToUserId != userId) 
                    throw new Exception("This task is not assigned to you.");
            }

            task.Status = request.Status;
            await _context.SaveChangesAsync();

            // Return a Response DTO instead of a string
            return _mapper.Map<TaskResponse>(task);
        }

        public async Task<string> DeleteTaskAsync(int id, int adminId)
        {
            //  Find the task and ensure we don't try to delete something already deleted
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id );

            if (task == null) return "NotFound";

            //  Ownership Check
            if (task.CreatedByUserId != adminId) return "Forbidden";

            if (task.Status == TMS.Model.Enums.TaskStatus.InProgress) return "InProgress";

            if (task.Status == TMS.Model.Enums.TaskStatus.Done) return "Completed";


            
            task.IsDeleted = true;

            
            await _context.SaveChangesAsync();

            return "Success";
        }
    }
}
