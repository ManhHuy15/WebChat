using BusinessObjects;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.GroupRepository
{
    public class GroupRepository : IGroupRepository
    {
        private GroupDAO _groupDAO;

        public GroupRepository(GroupDAO groupDAO)
        {
            _groupDAO = groupDAO;
        }

        public async Task<List<GroupMember>> GetGroupMemberByCondition(int userId) => await _groupDAO.GetGroupMemberByCondition(gm => gm.UserId == userId).ToListAsync();
        
    }
}
