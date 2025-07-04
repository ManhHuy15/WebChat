using BusinessObjects;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.FriendRepository
{
    public class FriendRepository : IFriendRepository
    {

        private FriendDAO _friendDAO;

        public FriendRepository(FriendDAO friendDAO)
        {
            _friendDAO = friendDAO;
        }

        public async Task<bool> AddFriend(FriendShip friendShip) => await _friendDAO.Add(friendShip);

        public async Task<bool> UpdateFriend(FriendShip friendShip) => await _friendDAO.Update(friendShip);

        public async Task<List<FriendShip>> getMyFriendships(int userId) =>  await _friendDAO.GetListFriendByCondition( f => f.UserId == userId && f.Status == (int) Enums.FriendShipStatus.Accepted).ToListAsync();

        public async Task<List<FriendShip>> getMyRequest(int userId) => await _friendDAO.GetListFriendByCondition(f => f.UserId == userId && f.Status == (int)Enums.FriendShipStatus.Pending).ToListAsync();

        public async Task<List<FriendShip>> getRequestToMe(int userId) => await _friendDAO.GetListFriendByCondition(f => f.FriendId == userId && f.Status == (int)Enums.FriendShipStatus.Pending).ToListAsync();

        public async Task<bool> RemoveFriend(int userId, int friendId) => await _friendDAO.Remove(userId, friendId);

      
    }
}
