using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.MessageRepository
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetChatList(int userId);

        Task<List<Message>> GetAllMessagesUser(int userId, int receiverId);
        Task AddRange(List<Message> messages );
    }
}
