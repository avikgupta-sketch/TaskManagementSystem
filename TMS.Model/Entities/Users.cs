using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Model.Enums;

namespace TMS.Model.Entities
{
    public class Users
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;

        // Navigation: Is user ne kaunse tasks handle kiye hain
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }
}
