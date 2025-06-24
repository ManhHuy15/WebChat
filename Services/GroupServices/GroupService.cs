using DTOs;
using DTOs.GroupDTOs;
using Repositories.GroupRepository;
using Repositories.MessageRepository;
using Services.ClouldinaryServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.GroupServices
{
    public class GroupService : IGroupService
    {
       
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<ResponseDTO<List<GroupBaseDTO>>> getMyGroups(int userId)
        {
            var groupMembers = await _groupRepository.GetMyGroupMember(userId);

            if (groupMembers == null)
            {
                return new ResponseDTO<List<GroupBaseDTO>>
                {
                    success = false,
                    message = "Not found",
                    data = new List<GroupBaseDTO>(),
                    status = HttpStatusCode.NotFound,
                };
            }

            var groupBase = groupMembers.Select(x => new GroupBaseDTO
            {
                Avatar = x.Group.Avatar,
                GroupId = x.GroupId,
                Name = x.Group.Name,
            }).ToList();

            return new ResponseDTO<List<GroupBaseDTO>>
            {
                success = true,
                message = "Success",
                data = groupBase,
                status = HttpStatusCode.OK,
            };
        }

    }
}
