using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UserRepository
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
    }
}
