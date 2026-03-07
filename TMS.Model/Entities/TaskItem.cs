using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = TMS.Model.Enums.TaskStatus;

namespace TMS.Model.Entities
{
    public class TaskItem :BaseEntity
    {
        
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } 
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        
        public int CreatedByUserId { get; set; } 
        public int AssignedToUserId { get; set; } 
        
       
        public DateTime? DueDate { get; set; }
        

        // Navigation
        public User CreatedBy { get; set; }
        public User? AssignedTo { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
