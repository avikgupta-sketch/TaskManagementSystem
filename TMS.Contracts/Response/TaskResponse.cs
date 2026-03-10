using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Contracts.Response
{
    internal class TaskResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? AssignedToUserName { get; set; } // We can simplify the response
        public DateTime CreatedAt { get; set; }
    }
}
