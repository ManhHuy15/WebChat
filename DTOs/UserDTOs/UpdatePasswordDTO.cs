using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.UserDTOs
{
    public class UpdatePasswordDTO : InitPasswordDTO
    {
        [Required(ErrorMessage = "Please enter your password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string CurrentPassword { get; set; }
    }
}
