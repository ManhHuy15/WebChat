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
    public class GroupDAO
    {
        private readonly WebChatContext _context;
        public GroupDAO(WebChatContext context)
        {
            _context = context;
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
    }
}
