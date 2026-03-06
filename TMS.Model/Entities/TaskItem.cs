using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = TMS.Model.Enums.TaskStatus;

namespace TMS.Model.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        // Foreign Keys
        public Guid CreatedById { get; set; } // Admin ki ID
        public Guid? AssignedToId { get; set; } // User ki ID (Nullable)

        // Navigation Properties
        public Users CreatedBy { get; set; }
        public Users AssignedTo { get; set; }

        public ICollection<Comments> Comments { get; set; } = new List<Comments>();
    }
}
