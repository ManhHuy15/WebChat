using BusinessObjects;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Repositories.GroupRepository
{
    public class GroupRepository : IGroupRepository
    {
        private GroupDAO _groupDAO;

        public GroupRepository(GroupDAO groupDAO)
        {
            _groupDAO = groupDAO;
        }
        public async Task<int> CreateGroup(Group group) => await _groupDAO.CreateGroup(group);

        public async Task<Group> GetDetails(int id) => await _groupDAO.GetGroupDetails(id);

        public async Task<List<GroupMember>> GetMyGroupMember(int userId) => await _groupDAO.GetGroupMemberByCondition(gm => gm.UserId == userId).ToListAsync();

        public async Task<bool> RemoveMemberFromGroup(int userId, int groupId) => await _groupDAO.RemoveMember(userId, groupId);

        public async Task<bool> AddMemberToGroup(List<GroupMember> groupMembers) => await _groupDAO.AddMember(groupMembers);
        public async Task<bool> UpdateGroup(Group group) => await _groupDAO.UpdateGroup(group);

        public async Task<List<GroupMember>> GetMembers(int groupId) => await _groupDAO.GetGroupMemberByCondition(gm => gm.GroupId == groupId).ToListAsync();

        public async Task<Group> GetCommonGroup(List<int> userIds) => await _groupDAO.GetCommonGroup(userIds);
    }
}
