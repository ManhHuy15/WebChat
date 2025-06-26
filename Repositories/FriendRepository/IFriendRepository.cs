using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.FriendRepository
{
    public interface IFriendRepository
    {
        Task<List<FriendShip>> getMyFriendships(int userId);

        Task<List<FriendShip>> getMyRequest(int userId);
        Task<List<FriendShip>> getRequestToMe(int userId);


    }
}
