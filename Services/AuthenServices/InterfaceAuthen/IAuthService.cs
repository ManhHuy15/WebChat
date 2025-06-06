using BusinessObjects;
using DTOs;
using DTOs.AuthenDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthenServices.InterfaceAuthen
{
    public interface IAuthService 
    {
        Task<ResponseDTO> LoginHandler(UserLoginRequestDTOs loginRequestDTOs);

        Task<ResponseDTO> RegisterHandler(RegisterUserDTOs registerUserDTOs);
    }
}
