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
    public class UserDAO
    {
        private readonly WebChatContext _context;
        public UserDAO(WebChatContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByCondition(Expression<Func<User, bool>> condition)
        {
            var user = new User();
            try
            {
                user = await _context.Users.Where(condition).FirstOrDefaultAsync();
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

        public async Task Update(User user)
        {
            try
            {
                _context.Entry<User>(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
