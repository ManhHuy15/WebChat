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
    public class UserDAO : BaseDAO
    {
        public UserDAO(WebChatContext context) : base(context)
        {
        }

        public async Task<User> GetUserByCondition(Expression<Func<User, bool>> condition)
        {
            var user = new User();
            try
            {
                user = await _context.Users.Where(condition).Include(u => u.ReceiverFriendShips)
                                                            .Include(u => u.SenderFriendShips)
                                                            .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user;
        }

        public async Task Add(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Update(User user)
        {
            try
            {
                _context.Entry<User>(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<User>> GetAll()
        {
            var users = new List<User>();
            try
            {
                users = await _context.Users.Where(x => x.IsActive == true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return users;
        }
    }
}
