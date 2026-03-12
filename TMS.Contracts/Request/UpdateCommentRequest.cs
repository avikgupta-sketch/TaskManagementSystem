using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Contracts.Request
{
    public class UpdateCommentRequest
    {
        public int CommentId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
