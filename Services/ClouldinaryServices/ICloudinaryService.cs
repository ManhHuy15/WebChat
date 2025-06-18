using CloudinaryDotNet.Actions;
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
        Task<string> UpLoadFileAsync(IFormFile file);
    }
}
