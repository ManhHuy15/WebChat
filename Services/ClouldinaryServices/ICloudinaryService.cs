using CloudinaryDotNet.Actions;
using DTOs.MessageDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ClouldinaryServices
{
    public interface ICloudinaryService
    {
        Task<CloudinryURLResponseDTO> UpLoadFileAsync(IFormFile file);
    }
}
