using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Model.Entities
{
    public class Comments
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }

        // Navigation Properties
        public TaskItem Task { get; set; }
        public Users Author { get; set; }
    }
}
