using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMS.Model.Enums;

namespace TMS.Model.Entities
{
    public class User : BaseEntity
    {
        
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        

        

        //Navigation
        public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
