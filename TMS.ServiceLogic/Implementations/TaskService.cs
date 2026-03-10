using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Contracts.Request;
using TMS.Model.Data;
using TMS.Model.Entities;
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


        public async Task<TaskItem> CreateTaskAsync(CreateTaskRequest request, int adminId)
        {
            if (request.AssignedToUserId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == request.AssignedToUserId.Value);

                if (!userExists)
                {
                    // If the user doesn't exist, we return null (or throw an exception)
                    return null;
                }
            }
            // request doesn't have AssignedToUserId, so it remains null in the entity
            var newTask = _mapper.Map<TaskItem>(request);

            newTask.CreatedByUserId = adminId;
            newTask.Status = TMS.Model.Enums.TaskStatus.Pending;

            _context.TaskItems.Add(newTask);
            await _context.SaveChangesAsync();
            return newTask;
        }

        public async Task<bool> AssignTaskAsync(AssignTaskRequest request)
        {
            var task = await _context.TaskItems.FindAsync(request.TaskId);
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);

            if (task == null || !userExists) return false;

            task.AssignedToUserId = request.UserId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
