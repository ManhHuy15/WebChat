using DTOs;
using DTOs.GroupDTOs;
using DTOs.UserDTOs;

namespace Services.GroupServices
{
    public interface IGroupService
    {
        Task<List<GroupBaseDTO>> getMyGroups(int userId);
        Task<ResponseDTO<GroupInfoDTO>> getDetails(int userId, int groupId);
        Task<ResponseDTO<bool>> RemoveMemberFromGroup(int userId, int groupId);
        Task<ResponseDTO<bool>> CreateGroup(GroupCreateDTO newGroup);
        Task<ResponseDTO<bool>> UpdateGroup(int groupId, GroupUpdateDTO group);
        Task<List<UserBaseDTO>> GetMembers(int groupId);
        Task<ResponseDTO<bool>> AddMembersToGroup(int groupId, AddMemberDTO members);
    }
}
