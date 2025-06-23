﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AuthenDTOs
{
    public class UserLoginResponseDTO
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int TokenExpires { get; set; }
    }
}
