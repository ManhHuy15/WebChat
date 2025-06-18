using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ClouldinaryServices
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly string TAGS = "Webchat";
        private readonly string FOLDER = "Webchat";
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary=cloudinary;
        }

        public async Task<string> UpLoadFileAsync(IFormFile file)
        {
            var result = new RawUploadResult();
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new RawUploadParams();
                if (file.ContentType.StartsWith("image/"))
                {
                    uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = FOLDER,
                        Tags = TAGS,
                        Transformation = new Transformation().Quality("auto")

                    };
                }
                else if (file.ContentType.StartsWith("video/"))
                {
                    uploadParams = new VideoUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = FOLDER,
                        Tags = TAGS,
                        Transformation = new Transformation().Quality("auto")
                    };
                }
                else
                {
                    uploadParams = new RawUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = FOLDER,
                        Tags = TAGS
                    };
                }

                result = await _cloudinary.UploadAsync(uploadParams);
            }
            if (result?.Url == null) return null;
            return result.Url.ToString();
        }
    }
}
