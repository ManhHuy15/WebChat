using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess
{
    public class GroupDAO : BaseDAO
    {
        public GroupDAO(WebChatContext context) : base(context)
        {
        }

        public IQueryable<GroupMember> GetGroupMemberByCondition(Expression<Func<GroupMember, bool>> condition)
        {
            try
            {
                return _context.GroupMembers.Include(gm => gm.Group)
                                                    .ThenInclude(g => g.Messages)
                                                        .ThenInclude(m => m.MessageStatuses)
                                                            .ThenInclude(ms => ms.User)
                                            .Include(gm => gm.User)
                                            .Where(condition).AsQueryable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Group> GetGroupDetails(int id)
        {
            try
            {
                return await _context.Groups.Include(g => g.Admin)
                                            .Include(g => g.GroupMembers)
                                            .FirstOrDefaultAsync(g => g.GroupId == id);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> RemoveMember(int userId, int groupId)
        {
            try
            {
                var groupMember = _context.GroupMembers.Where(gm => gm.GroupId == groupId && gm.UserId == userId).FirstOrDefault();
                if (groupMember == null)
                {
                    return false;
                }
                _context.GroupMembers.Remove(groupMember);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> CreateGroup(Group group)
        {
            try
            {
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();
                return group.GroupId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddMember(List<GroupMember> groupMembers)
        {
            try
            {
                _context.GroupMembers.AddRange(groupMembers);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateGroup(Group group)
        {
            try
            {
                var existingGroup = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == group.GroupId);
                if (existingGroup == null)
                {
                    return false;
                }

                existingGroup.Name = group.Name;
                existingGroup.Avatar = group.Avatar ?? existingGroup.Avatar;

                _context.Groups.Update(existingGroup);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
