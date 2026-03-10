using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Contracts.Request
{
    public class AssignTaskRequest
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
    }
}
