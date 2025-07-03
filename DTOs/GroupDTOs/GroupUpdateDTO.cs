using Microsoft.AspNetCore.Http;

namespace DTOs.GroupDTOs
{
    public class GroupUpdateDTO
    {
        public string Name { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
