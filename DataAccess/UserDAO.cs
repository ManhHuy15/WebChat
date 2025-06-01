using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<User> GetUsers()
        {
            var users = new List<User>();
            try
            {
                users = _context.Users.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return users;
        }
    }
}
