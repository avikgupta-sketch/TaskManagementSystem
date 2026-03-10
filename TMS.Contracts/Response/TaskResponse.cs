using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Contracts.Response
{
     public class TaskResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? AssignedToUsername { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
