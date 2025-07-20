using DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AuthenDTOs
{
    public class ResetPasswordDTO : InitPasswordDTO
    {
        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(150, ErrorMessage = "Email must be less than 150 characters")]
        public string Email { get; set; }

        public string? Otp { get; set; }
    }
}
