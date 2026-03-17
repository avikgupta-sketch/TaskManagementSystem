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
using static TMS.Model.Exceptions.Exceptions;

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
                    throw new NotFoundException("Assigned user not found.");

                if (assignedUser.Role != UserRole.User)
                    throw new ValidationException("Tasks can only be assigned to user.");

            }
            bool isDuplicate = await _context.TaskItems.AnyAsync(t =>
            t.Title == request.Title &&
            t.AssignedToUserId == request.AssignedToUserId &&
            t.CreatedByUserId == adminId && 
            !t.IsDeleted);

            if (isDuplicate)
                throw new DuplicateTaskException("You have already created a task with this title.");

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

        public async Task<bool> AssignTaskAsync(AssignTaskRequest request, int adminId)
        {

            var task = await _context.TaskItems.FindAsync(request.TaskId);
            var AssignedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            

            if (task == null ) throw new NotFoundException("Task not found.");
            if (AssignedUser==null) throw new NotFoundException("User not found.");
            if (AssignedUser.Role != UserRole.User) throw new ValidationException("Tasks can only be assigned to the 'User' role.");

            if (task.CreatedByUserId != adminId) throw new ValidationException("Access Denied: You can only assign tasks created by you.");
            if (task.AssignedToUserId != null) throw new DuplicateTaskException("This task is already assigned to a user.");


            task.AssignedToUserId = request.UserId;
            await _context.SaveChangesAsync();
            return true;
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
                throw new NotFoundException("Task not found.");

            // User can only see their assigned task
            if (role == "User" && task.AssignedToUserId != userId)
                throw new ForbiddenException("You are not authorized to view this task.");

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
                throw new NotFoundException("Task not found.");

            if (task.CreatedByUserId != userId)
                throw new ForbiddenException("You can only update tasks you created.");

            if (request.AssignedToUserId.HasValue)
            {
                var AssignedUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == request.AssignedToUserId.Value);
                if (AssignedUser==null)
                    throw new NotFoundException("The user you are trying to assign was not found.");
                if (AssignedUser.Role != UserRole.User)
                    throw new ValidationException("Tasks can only be assigned to users.");
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

            

            if (task == null) throw new NotFoundException("Task not found.");

            if (userRole == "Admin")
            {
                if (task.CreatedByUserId != userId) 
                    throw new ForbiddenException("You cannot update the status of a task you did not create.");
            }
            else
            {
                if (task.AssignedToUserId != userId) 
                    throw new ForbiddenException("You cannot update the status of a task not assigned to you.");
            }

            task.Status = request.Status;
            await _context.SaveChangesAsync();
           
            return _mapper.Map<TaskResponse>(task);
        }

        public async Task<bool> DeleteTaskAsync(int id, int adminId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id );

            if (task == null) throw new NotFoundException("Task not found.");

            //  Ownership Check
            if (task.CreatedByUserId != adminId) throw new ForbiddenException("Access Denied: You can only delete tasks created by you.");

            if (task.Status == TMS.Model.Enums.TaskStatus.InProgress) throw new ValidationException("Task cannot be deleted because it is currently in progress.");

            if (task.Status == TMS.Model.Enums.TaskStatus.Done) throw new ValidationException("Task cannot be deleted because it is already completed.");



            task.IsDeleted = true;

            
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
