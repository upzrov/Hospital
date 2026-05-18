
using BLL.DTOs;

namespace BLL.Services
{
    public interface IAuthService
    {
        Task<AuthResult> SignUpAsync(SignupDto model);
        Task<AuthResult> SignInAsync(SigninDto model);
    }
}
