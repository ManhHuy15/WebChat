using BusinessObjects;
using DTOs.MessageDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Services.UserServices;
using System.Security.Claims;

namespace ApiServices.Chat
{
    [Authorize]
    public class ChatHub : Hub
    {

        private readonly IUserService _userService;

        public ChatHub(IUserService userService)
        {
            this._userService = userService;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string senderId,string receiverId)
        {
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("AddToGroupResponse", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task SendMessageToGroup(string groupName, string groupId)
        {
            await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("ReceiveGroupMessage", groupId);
        }

        public async Task Notification(string receiverId, string message)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserById(int.Parse(userId), int.Parse(userId));
            var finalMessage = user.data.FullName + " " + message;
            await Clients.User(receiverId).SendAsync("ReceiveNotification", finalMessage);
        }
    }
}
