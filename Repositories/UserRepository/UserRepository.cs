using BusinessObjects;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<User> GetUser(Expression<Func<User, bool>> condition) => await _userDAO.GetUserByCondition(condition);

        public Task Add(User user) => _userDAO.Add(user);
        public Task Update(User user) => _userDAO.Update(user);

        public Task<List<User>> AllUser() => _userDAO.GetAll();
    }
}
