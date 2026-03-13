using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Model.Entities
{
    public class Comment : BaseEntity
    {

        public string Message { get; set; } = string.Empty;

        public int TaskItemId { get; set; }
        public int? UserId { get; set; }

        // Navigation Properties
        public TaskItem Task { get; set; }
        public User Author { get; set; }
        
    }
}
