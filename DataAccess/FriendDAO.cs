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
    public class FriendDAO : BaseDAO
    {
        public FriendDAO(WebChatContext context) : base(context)
        {
        }

        public IQueryable<FriendShip> GetListFriendByCondition(Expression<Func<FriendShip, bool>> condition)
        {
            try
            {
                return _context.FriendShips.Include(f => f.User)
                                            .Include(f => f.Friend)
                                            .Where(condition).AsQueryable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
