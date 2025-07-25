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

        public async Task<ResponseDTO<GroupBaseDTO>> CreateGroup(GroupCreateDTO g)
        {
            var userIds = g.MemberIds.ToList();
            userIds.Add(g.AdminId);
            var group = await _groupRepository.GetCommonGroup(userIds);

            if (group != null)
            {
                return new ResponseDTO<GroupBaseDTO>
                {
                    status = HttpStatusCode.Forbidden,
                    message = $"A group with the same members already exists. You can use the {group.Name} group instead of creating a new one.",
                    success = false,
                    data = new GroupBaseDTO
                    {
                        Avatar = group.Avatar,
                        GroupId = group.GroupId,
                        Name = group.Name
                    }
                };
            }

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
                return new ResponseDTO<GroupBaseDTO>
                {
                    status = HttpStatusCode.OK,
                    message = "Fail to create group",
                    success = false,
                    data = new GroupBaseDTO()
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
                return new ResponseDTO<GroupBaseDTO>
                {
                    status = HttpStatusCode.OK,
                    message = "Fail to add member to new group",
                    success = false,
                    data = new GroupBaseDTO()
                };
            }

            return new ResponseDTO<GroupBaseDTO>
            {
                status = HttpStatusCode.OK,
                message = "Create group successfully",
                success = true,
                data = new GroupBaseDTO
                {
                    Avatar = newGroup.Avatar,
                    GroupId = newGroupId,
                    Name = newGroup.Name
                }
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
            var groupMember = await _groupRepository.GetMembers(groupId);

            if(groupMember.Count == 2)
            {
                return new ResponseDTO<bool>
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Your group has only two members left.",
                    success = false,
                    data = false
                };
            }

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

        public async Task<ResponseDTO<GroupBaseDTO>> AddMembersToGroup(int groupId, AddMemberDTO members)
        {

            var group = await _groupRepository.GetDetails(groupId);
            var userIds = group.GroupMembers.Select(x => x.UserId).ToList();
            userIds.AddRange(members.UserIds);
            var Commongroup = await _groupRepository.GetCommonGroup(userIds);

            if (Commongroup != null)
            {
                return new ResponseDTO<GroupBaseDTO>
                {
                    status = HttpStatusCode.Forbidden,
                    message = $"A group with the same members already exists. You can use the {Commongroup.Name} group instead of creating a new one.",
                    success = false,
                    data = new GroupBaseDTO
                    {
                        Avatar = Commongroup.Avatar,
                        GroupId = Commongroup.GroupId,
                        Name = Commongroup.Name
                    }
                };
            }

            var memberGroup = members.UserIds.Select(x => new GroupMember
            {
                GroupId = groupId,
                UserId = x,
            }).ToList();

            var result = await _groupRepository.AddMemberToGroup(memberGroup);

            var res = new ResponseDTO<GroupBaseDTO>
            {
                status = HttpStatusCode.OK,
                message = result ? "Success" : "Fail",
                success = result,
                data = new GroupBaseDTO
                {
                    Avatar = group.Avatar,
                    GroupId = group.GroupId,
                    Name = group.Name
                }
            };
            return res;
        }
    }
}
