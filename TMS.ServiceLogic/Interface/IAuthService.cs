using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  TMS.Contracts.Response;
using  TMS.Contracts.Request;

namespace TMS.ServiceLogic.Interface
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);

        Task<AuthResponse?> LoginAsync(LoginRequest request);

        Task<bool> DeleteUserAsync(int userId);
    }
}
