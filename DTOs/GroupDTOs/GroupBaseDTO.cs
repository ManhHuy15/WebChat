using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.GroupDTOs
{
    public class GroupBaseDTO
    {
        public int? GroupId { get; set; }
        public string? Avatar { get; set; }
        public string Name { get; set; }
    }
}
