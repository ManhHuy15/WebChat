using BusinessObjects;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private UserDAO _userDAO;

        public UserRepository( UserDAO userDAO)
        {
            _userDAO = userDAO;
        }

        public List<User> GetAllUsers()
        {
            return _userDAO.GetUsers();
        }
    }
}
