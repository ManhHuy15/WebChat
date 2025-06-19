using DTOs;
using DTOs.MessageDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MessageServices
{
    public interface IMessageService
    {
        Task<ResponseDTO<List<ChatItemDTO>>> GetListChat(int userId);

        Task<ResponseDTO<List<MessageUserDTO>>> GetAllMessagesUser(int userId, int receiverId);

        Task<ResponseDTO<string>> SendMessage(int userId, SendMessageDTO message);

    }
}
