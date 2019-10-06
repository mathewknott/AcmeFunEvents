using System.Threading.Tasks;
using AcmeFunEvents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AcmeFunEvents.Web.Pages.Activities
{
    public class IndexModel : PageModel
    {
        private UserManager<ApplicationUser> UserManager { get; }
        
        private IAuthorizationService AuthorizationService { get; }
             
        public IndexModel(UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService)
        {
            UserManager = userManager;
            AuthorizationService = authorizationService;
        }
 
        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await UserManager.GetUserAsync(User);
            if (currentUser == null) return Page();                       
            return Page();
        }
    }
}