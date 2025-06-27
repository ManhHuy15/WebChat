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


        public async Task<bool> Add(FriendShip friend)
        {
            try
            {
                var check = _context.FriendShips.Where(f => f.UserId == friend.UserId && f.FriendId == friend.FriendId).FirstOrDefault();
                if (check != null) {
                    return false;
                }
                _context.FriendShips.Add(friend);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Update(FriendShip friend)
        {
            try
            {
                var existingFriendShip = await _context.FriendShips.FirstOrDefaultAsync(f => f.UserId == friend.UserId && f.FriendId == friend.FriendId);
                if (existingFriendShip == null)
                {
                    return false;
                }

                existingFriendShip.Status = friend.Status;

                _context.Entry(existingFriendShip).CurrentValues.SetValues(friend);
                await _context.SaveChangesAsync();
                return true;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task<bool> Remove(int userId, int friendId)
        {
            try
            {
                var friend = _context.FriendShips.Where(f =>( f.UserId == userId && f.FriendId == friendId)
                                                            || (f.UserId == friendId && f.FriendId == userId));

                if (friend == null)
                { 
                    return false;
                }
                _context.FriendShips.RemoveRange(friend);
                await _context.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
