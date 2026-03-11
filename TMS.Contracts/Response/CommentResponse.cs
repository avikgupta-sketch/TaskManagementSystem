using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Contracts.Response
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TaskItemId { get; set; }
        public int UserId { get; set; }
        public string AuthorName { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
    }
}

