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
        Task<TaskItem?> CreateTaskAsync(CreateTaskRequest request, int adminId);

        Task<string> AssignTaskAsync(AssignTaskRequest request,int adminId);
        Task<List<TaskResponse>> GetAllTasksAsync(int userId, string role);
        Task<TaskResponse> GetTaskByIdAsync(int taskId, int userId, string role);
        Task<TaskResponse> UpdateTaskAsync(int taskId, UpdateTaskRequest request, int userId);

        Task<TaskResponse?> UpdateTaskStatusAsync(UpdateTaskStatusRequest request, int userId, string userRole);

        Task<string> DeleteTaskAsync(int id, int adminId);
    }
}
