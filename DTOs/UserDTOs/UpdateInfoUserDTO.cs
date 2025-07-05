using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.UserDTOs
{
    public class UpdateInfoUserDTO
    {
        public string? Phone { get; set; }
        public DateOnly? Birth { get; set; }
        public bool? Gender { get; set; }
    }
}
