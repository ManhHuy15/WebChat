using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AuthenDTOs
{
    public class RegisterUserDTOs
    {

        [Required(ErrorMessage = "Please enter your full name")]
        [MaxLength(150, ErrorMessage = "Full name must be less than 150 characters")]
        [MinLength(3, ErrorMessage = "Full name must be at least 3 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(150, ErrorMessage = "Email must be less than 150 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your birthday")]
        public DateOnly Birth { get; set; }

        [Required(ErrorMessage = "Please choose your gender")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }
}
