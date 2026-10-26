using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Application.DTOs.Auth;

namespace EMS.Application.Interfaces
{
    public interface IAuthService       // this is what the AuthController will actually depend on directly
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}