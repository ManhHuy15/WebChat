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
    public interface IUserRepository
    {
        Task<User> GetUser(Expression<Func<User, bool>> condition);
        Task Add(User user);
        Task Update(User user);
    }
}
