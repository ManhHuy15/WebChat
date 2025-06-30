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
        Task<List<GroupBaseDTO>> getMyGroups(int userId);
        Task<ResponseDTO<GroupInfoDTO>> getDetails(int userId,int groupId);
        Task<ResponseDTO<bool>> RemoveMemberFromGroup(int userId, int groupId);
        Task<ResponseDTO<bool>> CreateGroup(GroupCreateDTO newGroup);
    }
}
