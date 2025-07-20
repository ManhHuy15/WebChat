using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AuthenDTOs
{
    public class VerifyOTPRequestDTO
    {
        public string? Email { get; set; }
        public string? Otp { get; set; }

    }
}
