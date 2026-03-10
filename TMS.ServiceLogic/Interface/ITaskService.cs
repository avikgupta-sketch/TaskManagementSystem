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
    }
}
