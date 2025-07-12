using BusinessObjects;
using DTOs;
using DTOs.GroupDTOs;
using DTOs.MessageDTOs;
using DTOs.UserDTOs;
using Repositories.GroupRepository;
using Repositories.MessageRepository;
using Repositories.UserRepository;
using Services.ClouldinaryServices;
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
        private readonly IGroupRepository _groupRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public MessageService(IMessageRepository messageRepository, ICloudinaryService cloudinaryService, IGroupRepository groupRepository)
        {
            _messageRepository = messageRepository;
            _cloudinaryService = cloudinaryService;
            _groupRepository = groupRepository;
        }

        public async Task<ResponseDTO<List<MessageGroupDTO>>> GetAllMessagesInGroup(int groupId)
        {
            var messages = await _messageRepository.GetAllMessagesInGroup(groupId);

            if (messages == null || !messages.Any())
            {
                return new ResponseDTO<List<MessageGroupDTO>>
                {
                    status = HttpStatusCode.OK,
                    message = "No messages found",
                    success = false,
                    data = new List<MessageGroupDTO>()
                };
            }

            var result = messages.Select(x => new MessageGroupDTO
            {

                Content = x.Content,
                SentAt = x.SentAt,
                Type = x.Type,
                Sender = new UserBaseDTO
                {
                    UserId = x.SenderId,
                    FullName = x.Sender.FullName,
                    Avatar = x.Sender.Avatar,
                    Email = x.Sender.Email
                },
                Group = new GroupBaseDTO
                {
                    GroupId = x.GroupId,
                    Name = x.Group.Name,
                    Avatar = x.Group.Avatar
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

            return new ResponseDTO<List<MessageGroupDTO>>
            {
                status = HttpStatusCode.OK,
                message = "Get all messages success",
                success = true,
                data = result
            };
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
            var groupList = await _groupRepository.GetMyGroupMember(userId);

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
            var groupItems = groupList.Where(x => x.Group.Messages.Any())
                                        .Select(x =>
                                        {
                                            var latestMessage = x.Group.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
                                            return new ChatItemDTO
                                            {
                                                Id = x.GroupId,
                                                Name = x.Group.Name,
                                                Avatar = x.Group.Avatar,
                                                ContentPreview = latestMessage?.Content,
                                                Time = latestMessage?.SentAt,
                                                Type = (int)Enums.ChatItemType.Group,
                                                IsRead = false,
                                            };
                                        })
                                        .OrderByDescending(x => x.Time)
                                        .ToList();

            chatList.AddRange(groupItems);

            chatList = chatList.OrderByDescending(x => x.Time).ToList();

            return new ResponseDTO<List<ChatItemDTO>>
            {
                status = HttpStatusCode.OK,
                message = "Get list chat success",
                success = true,
                data = chatList
            };
        }


        public async Task<ResponseDTO<List<MessageUserDTO>>> GetUserMessageFile(int userId, int receiverId)
        {
            var messages = await _messageRepository.GetUserMessageFile(userId, receiverId);

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
            }).ToList();

            return new ResponseDTO<List<MessageUserDTO>>
            {
                status = HttpStatusCode.OK,
                message = "Get user message files success",
                success = true,
                data = result
            };
        }

        public async Task<ResponseDTO<string>> SendMessage(int userId,int type, SendMessageDTO message)
        {
            List<Message> messages = new List<Message>();

            if (message.Files != null && message.Files.Any())
            {
                foreach (var file in message.Files)
                {
                    var result = await _cloudinaryService.UpLoadFileAsync(file);
                    if (result == null) continue;
                    messages.Add(new Message
                    {
                        Content = result.Url,
                        Type = result.Type,
                        SenderId = userId,
                        ReceiverId = type == (int)Enums.ChatItemType.User ? message.ReceiverId : null,
                        GroupId = type == (int)Enums.ChatItemType.Group ? message.ReceiverId : null
                    });
                }
            }
            if (!string.IsNullOrEmpty(message.Content))
            {
                messages.Add(new Message
                {
                    Content = message.Content,
                    Type = (int)Enums.MessageType.Text,
                    SenderId = userId,
                    ReceiverId = type == (int)Enums.ChatItemType.User ? message.ReceiverId : null,
                    GroupId = type == (int)Enums.ChatItemType.Group ? message.ReceiverId : null
                });
            }

            await _messageRepository.AddRange(messages);

            return new ResponseDTO<string>
            {
                status = HttpStatusCode.OK,
                message = "Send message success",
                success = true,
            };
        }
    }
}
