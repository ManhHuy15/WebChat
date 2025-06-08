using DTOs.AuthenDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public UserLoginRequestDTO loginUser { get; set; }
        public void OnGet()
        {
        }
    }
}
