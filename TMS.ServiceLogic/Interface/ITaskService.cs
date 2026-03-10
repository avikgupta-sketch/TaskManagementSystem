using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Contracts.Request;
using TMS.Model.Entities;
using TMS.Contracts.Response;

namespace TMS.ServiceLogic.Interface
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(CreateTaskRequest request, int adminId);

        Task<bool> AssignTaskAsync(AssignTaskRequest request);
        Task<List<TaskResponse>> GetAllTasksAsync(int userId, string role);
        Task<TaskResponse> GetTaskByIdAsync(int taskId, int userId, string role);
        Task<TaskResponse> UpdateTaskAsync(int taskId, UpdateTaskRequest request, int userId);
    }
}
