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
        public async Task<List<Message>> GetAllMessages(int userId)
        {
            var messages = await _messageDAO.GetListMessageByCondition(x => x.SenderId == userId || x.ReceiverId == userId)
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
    }
}