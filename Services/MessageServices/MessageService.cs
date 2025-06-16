using BusinessObjects;
using DTOs;
using DTOs.MessageDTOs;
using DTOs.UserDTOs;
using Repositories.MessageRepository;
using Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async Task<ResponseDTO<List<MessageUserDTO>>> GetAllMessagesUser(int userId, int receiverId)
        {
           var messages = await _messageRepository.GetAllMessagesUser(userId, receiverId);

            if (messages == null || !messages.Any())
            {
                return new ResponseDTO<List<MessageUserDTO>>
                {
                    status = HttpStatusCode.OK,
                    message = "No messages found",
                    success = false,
                    data = new List<MessageUserDTO>()
                };
            }

            var result = messages.Select(x => new MessageUserDTO
            {
                
                Content = x.Content,
                SentAt = x.SentAt,
                Type = x.Type,
                Sender = new UserBaseDTO
                {
                    FullName = x.Sender.FullName,
                    Avatar = x.Sender.Avatar
                },
                Receiver = new UserBaseDTO
                {
                    UserId = x.ReceiverId,
                    FullName = x.Receiver.FullName,
                    Avatar = x.Receiver.Avatar
                },
                MessageStatuses = x.MessageStatuses.Select(ms => new MessageStatusDTO
                {
                    IsRead = ms.IsRead,
                    UpdatedAt = ms.UpdatedAt,
                    User = new UserBaseDTO
                    {
                        FullName = ms.User.FullName,
                        Avatar = ms.User.Avatar
                    }
                }).ToList()
            }).ToList();

            return new ResponseDTO<List<MessageUserDTO>>
            {
                status = HttpStatusCode.OK,
                message = "Get all messages success",
                success = true,
                data = result
            };
        }

        public async Task<ResponseDTO<List<ChatItemDTO>>> GetListChat(int userId)
        {
            var messages = await _messageRepository.GetChatList(userId);

            if (messages == null || !messages.Any())
            {
                return new ResponseDTO<List<ChatItemDTO>>
                {
                    status = HttpStatusCode.OK,
                    message = "No messages found",
                    success = false,
                    data = new List<ChatItemDTO>()
                };
            }


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

            return new ResponseDTO<List<ChatItemDTO>>
            {
                status = HttpStatusCode.OK,
                message = "Get list chat success",
                success = true,
                data = chatList
            };
        }
    }
}
