using DTOs.AuthenDTOs;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages
{
    public class ResetPasswordModel : PageModel
    {

        [BindProperty]
        public InitPasswordDTO Password { get; set; }
        public void OnGet()
        {
        }
    }
}
