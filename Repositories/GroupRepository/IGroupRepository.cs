using BusinessObjects;

namespace Repositories.GroupRepository
{
    public interface IGroupRepository
    {
        Task<Group> GetDetails(int id);
        Task<List<GroupMember>> GetMyGroupMember(int userId);
        Task<bool> RemoveMemberFromGroup(int userId, int groupId);
        Task<int> CreateGroup(Group group);
        Task<bool> AddMemberToGroup(List<GroupMember> groupMembers);
        Task<bool> UpdateGroup(Group group);
        Task<List<GroupMember>> GetMembers(int groupId);

        Task<Group> GetCommonGroup(List<int> userIds);
    }
}
