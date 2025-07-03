using BusinessObjects;
using DTOs;
using DTOs.GroupDTOs;
using DTOs.MessageDTOs;
using DTOs.UserDTOs;
using Repositories.GroupRepository;
using Repositories.MessageRepository;
using Services.ClouldinaryServices;
using System.Net;

namespace Services.GroupServices
{
    public class GroupService : IGroupService
    {
        private readonly string AVATAR_GROUP_DEFAULT = "http://res.cloudinary.com/ddg2gdfee/image/upload/v1751255579/Webchat/groupdefault_rufg7z.jpg";
        private readonly IGroupRepository _groupRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public GroupService(IGroupRepository groupRepository, ICloudinaryService cloudinaryService, IMessageRepository messageRepository)
        {
            _groupRepository = groupRepository;
            _cloudinaryService=cloudinaryService;
            _messageRepository=messageRepository;
        }

        public async Task<ResponseDTO<bool>> CreateGroup(GroupCreateDTO g)
        {

            var newGroup = new Group()
            {
                AdminId = g.AdminId,
                CreatedAt = DateTime.Now,
                Name = g.Name,
                IsPrivate = g.IsPrivate
            };

            if (g.Avatar != null)
            {
                var res = await _cloudinaryService.UpLoadFileAsync(g.Avatar);
                newGroup.Avatar = res.Url;
            }
            else
            {
                newGroup.Avatar = AVATAR_GROUP_DEFAULT;
            }
            var newGroupId = await _groupRepository.CreateGroup(newGroup);

            if (newGroupId < 0)
            {
                return new ResponseDTO<bool>
                {
                    status = HttpStatusCode.OK,
                    message = "Fail to create group",
                    success = false,
                    data = false
                };
            }

            List<GroupMember> groupMembers = new List<GroupMember>()
            {
                new GroupMember
                {
                    GroupId = newGroupId,
                    UserId = g.AdminId,
                }
            };

            foreach (var memberId in g.MemberIds)
            {
                groupMembers.Add(new GroupMember()
                {
                    GroupId = newGroupId,
                    UserId = memberId,
                });
            }

            var result = await _groupRepository.AddMemberToGroup(groupMembers);

            if (!result)
            {
                return new ResponseDTO<bool>
                {
                    status = HttpStatusCode.OK,
                    message = "Fail to add member to new group",
                    success = false,
                    data = false
                };
            }

            return new ResponseDTO<bool>
            {
                status = HttpStatusCode.OK,
                message = "Create group successfully",
                success = true,
                data = true
            };
        }

        public async Task<ResponseDTO<GroupInfoDTO>> getDetails(int userId, int groupId)
        {
            var group = await _groupRepository.GetDetails(groupId);
            if (group == null)
            {
                return new ResponseDTO<GroupInfoDTO>
                {
                    status = HttpStatusCode.NotFound,
                    message = "Group not found",
                    success = false,
                    data = new GroupInfoDTO()
                };
            }

            var messageFile = await _messageRepository.GetMessagesFileInGroup(groupId);

            var ressult = new GroupInfoDTO
            {
                Avatar = group.Avatar,
                GroupId = group.GroupId,
                Name = group.Name,
                CreatedAt = group.CreatedAt,
                IsPrivate = group.IsPrivate,
                Admin = new UserBaseDTO()
                {
                    Avatar = group.Admin.Avatar,
                    FullName = group.Admin.FullName,
                },
                memberCount = group.GroupMembers.Count,
                isAdmin = group.AdminId == userId,
                Messages = messageFile.Any() ? messageFile.Select(x => new MessageDTO
                {
                    Type = x.Type,
                    Content = x.Content,
                }).ToList()
                                                : new List<MessageDTO>(),
            };


            return new ResponseDTO<GroupInfoDTO>
            {
                status = HttpStatusCode.OK,
                message = "Success",
                success = true,
                data = ressult
            };
        }

        public async Task<List<UserBaseDTO>> GetMembers(int groupId)
        {
            var members = await _groupRepository.GetMembers(groupId);

            if (members == null)
            {
                return new List<UserBaseDTO>();
           
            }

            var result = members.Select(x => new UserBaseDTO
            {
                Avatar = x.User.Avatar,
                FullName = x.User.FullName,
                UserId = x.User.UserId,
                Email = x.User.Email,
            }).ToList();

            return result;
        }

        public async Task<List<GroupBaseDTO>> getMyGroups(int userId)
        {
            var groupMembers = await _groupRepository.GetMyGroupMember(userId);

            if (groupMembers == null)
            {
                return new List<GroupBaseDTO>();
            }

            var groupBase = groupMembers.Select(x => new GroupBaseDTO
            {
                Avatar = x.Group.Avatar,
                GroupId = x.GroupId,
                Name = x.Group.Name,
            }).ToList();

            return groupBase;
        }

        public async Task<ResponseDTO<bool>> RemoveMemberFromGroup(int userId, int groupId)
        {
            var result = await _groupRepository.RemoveMemberFromGroup(userId, groupId);
            return new ResponseDTO<bool>
            {
                status = HttpStatusCode.OK,
                message = result ? "Success" : "Fail",
                success = result,
                data = result
            };
        }

        public async Task<ResponseDTO<bool>> UpdateGroup(int groupId, GroupUpdateDTO group)
        {

            var groupUpdate = new Group
            {
                GroupId = groupId,
                Name = group.Name,
            };

            if (group.Avatar != null)
            {
                var res = await _cloudinaryService.UpLoadFileAsync(group.Avatar);
                groupUpdate.Avatar = res.Url;
            }

            var result = await _groupRepository.UpdateGroup(groupUpdate);

            return new ResponseDTO<bool>
            {
                status = HttpStatusCode.OK,
                message = result ? "Update Success" : "Update Fail",
                success = result,
                data = result
            };

        }

        public async Task<ResponseDTO<bool>> AddMembersToGroup(int groupId, AddMemberDTO members)
        {
            var memberGroup = members.UserIds.Select(x => new GroupMember
            {
                GroupId = groupId,
                UserId = x,
            }).ToList();

            var result = await _groupRepository.AddMemberToGroup(memberGroup);

            var res = new ResponseDTO<bool>
            {
                status = HttpStatusCode.OK,
                message = result ? "Success" : "Fail",
                success = result,
                data = result
            };
            return res;
        }
    }
}
