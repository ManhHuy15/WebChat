using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.UserDTOs
{
    public class UpdateAvatarDTO
    {
        public IFormFile? Avatar { get; set; }
    }
}
