using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = TMS.Model.Enums.TaskStatus;

namespace TMS.Contracts.Request
{
    public class UpdateTaskStatusRequest
    {
        public int TaskId { get; set; }

        public TaskStatus Status { get; set; }
    }
}
