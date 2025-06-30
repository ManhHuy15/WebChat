using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
    }
}
