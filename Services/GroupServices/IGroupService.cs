using DTOs;
using DTOs.GroupDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.GroupServices
{
    public interface IGroupService
    {
        Task<ResponseDTO<List<GroupBaseDTO>>> getMyGroups(int userId);
    }
}
