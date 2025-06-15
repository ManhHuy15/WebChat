using BusinessObjects;
using DTOs;
using Repositories.MessageRepository;
using Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MessageServices
{
    public class MessageService : IMessageService
    {

        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<List<ChatItemDTO>> GetListChat(int userId)
        {
            var messages = await _messageRepository.GetAllMessages(userId);

            var chatList = messages.Select(x => new ChatItemDTO
            {
                Id = x.SenderId == userId ? x.ReceiverId : x.SenderId,
                Name = x.SenderId == userId ? x.Receiver.FullName : x.Sender.FullName,
                Avatar = x.SenderId == userId ? x.Receiver.Avatar : x.Sender.Avatar,
                ContentPreview = x.Content,
                Time = x.SentAt,
                Type = (int)Enums.ChatItemType.User,
                IsRead = false
            }).OrderByDescending(m => m.Time).ToList();


            //Ket hợp với danh sách nhóm chat

            return chatList != null ? chatList : new List<ChatItemDTO>();
        }
    }
}
