using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Model.Exceptions
{
    public class Exceptions
    {
        //  404
        public class NotFoundException : Exception { public NotFoundException(string msg) : base(msg) { } }

        // 409 (Conflict)
        public class DuplicateTaskException : Exception { public DuplicateTaskException(string msg) : base(msg) { } }

        // 403 or 400
        public class ValidationException : Exception { public ValidationException(string msg) : base(msg) { } }

        public class ForbiddenException : Exception{ public ForbiddenException(string msg) : base(msg) { } }
    }
}
