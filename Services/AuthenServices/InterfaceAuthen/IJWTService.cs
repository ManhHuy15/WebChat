using BusinessObjects;
using DTOs.AuthenDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthenServices.InterfaceAuthen
{
    public interface IJWTService
    {
        Task<UserLoginResponseDTO> AuthenticateUser(User loginUser, bool isRefreshToken);
        string GenerateRefreshToken();
        string GenerateJwtToken(User user);
    }
}
