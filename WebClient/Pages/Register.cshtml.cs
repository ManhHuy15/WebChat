using DTOs.AuthenDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterUserDTOs RegisterUser { get; set; }
        public void OnGet()
        {
        }
    }
}
