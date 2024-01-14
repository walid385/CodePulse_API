using CodePulse.API.Models;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using Identity.Models;
using Microsoft.AspNetCore.Mvc;


namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SendMail([FromBody] ContactUsDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = new ContactUsDto
            {
                Email = request.Email,
                Message = request.Message,
                Name = request.Name,
            };

            EmailHelperContactUs contactUs = new EmailHelperContactUs();
            bool emailResponse = contactUs.SendEmail(request.Name, request.Message, request.Email);

            // Check if the email was sent successfully
            if (!emailResponse)
            {
                // If email sending failed, add an error to ModelState and return a validation problem
                ModelState.AddModelError("", "Failed to send confirmation email.");
                return ValidationProblem(ModelState);
            }

            return Ok(message);


        }
    }
}

