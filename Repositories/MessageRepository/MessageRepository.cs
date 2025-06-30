using BusinessObjects;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.MessageRepository
{
    public class MessageRepository : IMessageRepository
    {

        private MessageDAO _messageDAO;

        public MessageRepository(MessageDAO messageDAO)
        {
            _messageDAO = messageDAO;
        }
        public async Task<List<Message>> GetChatList(int userId)
        {
            var messages = await _messageDAO.GetListMessageByCondition(x => (x.SenderId == userId && x.ReceiverId != null ) 
                                                                            || x.ReceiverId == userId)
                                            .Select(m => new
                                            {
                                                Message = m,
                                                OtherUserId = m.SenderId == userId ? m.ReceiverId : m.SenderId
                                            })
                                            .GroupBy(x => x.OtherUserId)
                                            .Select(g => g.OrderByDescending(x => x.Message.SentAt).First().Message)
                                            .ToListAsync();
            return messages;
        }
        public async Task<List<Message>> GetAllMessagesUser(int userId, int receiverId)
        {
            return await _messageDAO.GetListMessageByCondition(x => 
                    (x.SenderId == userId && x.ReceiverId == receiverId) 
                    || (x.SenderId == receiverId && x.ReceiverId == userId)
            ).ToListAsync();
        }
        public Task AddRange(List<Message> messages) => _messageDAO.AddRange(messages);

        public Task<List<Message>> GetAllMessagesInGroup(int groupId) => _messageDAO.GetListMessageByCondition(x => x.GroupId == groupId).ToListAsync();

        public Task<List<Message>> GetMessagesFileInGroup(int groupId) 
            => _messageDAO.GetListMessageByCondition(x => x.GroupId == groupId && (x.Type == (int) Enums.MessageType.Video 
                                                                                || x.Type == (int)Enums.MessageType.Image
                                                                                || x.Type == (int)Enums.MessageType.File
                                                                                || x.Type == (int)Enums.MessageType.Audio)).ToListAsync();
    }
}