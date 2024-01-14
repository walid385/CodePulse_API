using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.EntityFrameworkCore;
using CodePulse.API.Data; 

namespace Identity.Controllers
{
    public class EmailController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly AuthDbContext context; 

        public EmailController(UserManager<IdentityUser> userManager, AuthDbContext ctx) 
        {
            this.userManager = userManager;
            this.context = ctx;
        }

        [HttpGet] 
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "No User Found");
                return NotFound(ModelState);
            }

            // Decode the token from the URL
            var decodedToken = WebEncoders.Base64UrlDecode(token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await userManager.ConfirmEmailAsync(user, normalToken);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }

            await context.SaveChangesAsync(); 

            return Ok(new { Result = "Email confirmed successfully." });
        }
    }
}
