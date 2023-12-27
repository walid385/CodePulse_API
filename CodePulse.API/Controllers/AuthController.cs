using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;
using Identity.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager,
            ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // Check if the request, email, or password is null or empty
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email or password cannot be empty.");
            }

            // Attempt to find the user by email
            var identityUser = await userManager.FindByEmailAsync(request.Email);

            // Check if the user was found
            if (identityUser == null)
            {
                // Return a generic error message to avoid enumeration attacks
                return BadRequest("Email or Password Incorrect");
            }

            // Check if the email is confirmed
            var isEmailConfirmed = await userManager.IsEmailConfirmedAsync(identityUser);
            if (!isEmailConfirmed)
            {
                return BadRequest("Please confirm your email first");
            }

            // Check the password
            var checkPasswordResult = await userManager.CheckPasswordAsync(identityUser, request.Password);
            if (!checkPasswordResult)
            {
                // Return a generic error message to avoid enumeration attacks
                return BadRequest("Email or Password Incorrect");
            }

            // If the user is found, the email is confirmed, and the password is correct, proceed to generate the token
            var roles = await userManager.GetRolesAsync(identityUser);
            var jwtToken = tokenRepository.CreateJwtToken(identityUser, roles.ToList());

            var response = new LoginResponseDto()
            {
                Email = request.Email,
                Roles = roles.ToList(),
                Token = jwtToken
            };

            return Ok(response);
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            // Validate the incoming request data
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Create a new user with the email from the request
            var user = new IdentityUser
            {
                UserName = request.Email?.Trim(),
                Email = request.Email?.Trim(),
            };

            // Attempt to create the user with the specified password
            var identityResult = await userManager.CreateAsync(user, request.Password);
            if (identityResult.Succeeded)
            {
                // Add the user to a default role, e.g., "Reader"
                await userManager.AddToRoleAsync(user, "Reader");

                // Generate an email confirmation token
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                // Create a confirmation link
                var codeEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var callbackUrl = $"{this.Request.Scheme}://localhost:4200/confirm-email?token={codeEncoded}&email={WebUtility.UrlEncode(user.Email)}";
                if (string.IsNullOrEmpty(callbackUrl))
                {
                    // Log the error or handle accordingly
                    throw new InvalidOperationException("The confirmation link could not be generated.");
                }

                // Send the email
                EmailHelperConfirmEmail emailHelper = new EmailHelperConfirmEmail();
                bool emailResponse = emailHelper.SendEmail(user.Email, callbackUrl);

                // Check if the email was sent successfully
                if (!emailResponse)
                {
                    // If email sending failed, add an error to ModelState and return a validation problem
                    ModelState.AddModelError("", "Failed to send confirmation email.");
                    return ValidationProblem(ModelState);
                }

                // If email sent successfully, return the user information
                user.EmailConfirmed = true;
                return Ok(new JsonResult(new { title = "Thank you!", message = "Please confirm your email now." }));
            }
            else
            {
                // If user creation failed, add each error to the ModelState
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return ValidationProblem(ModelState);
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("This email adress has not been registered yet");

            if (user.EmailConfirmed == true) return BadRequest("Your email is already confirmed, please login.");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Email Confirmed", message = "Your email is confirmed. You can login now." }));
                }

                return BadRequest("Invalid Token");
            }
            catch (Exception)
            {
                return BadRequest("Invalid Token");
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return BadRequest("Invalid Request");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string?>
            {
                {"token", token },
                {"email", forgotPasswordDto.Email }
            };

            var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientURI, param);

            // Send the email
            EmailHelperForgotPw emailHelper = new EmailHelperForgotPw();
            bool emailResponse = emailHelper.SendEmail(user.Email, callback);

            // Check if the email was sent successfully
            if (!emailResponse)
            {
                // If email sending failed, add an error to ModelState and return a validation problem
                ModelState.AddModelError("", "Failed to send reset email.");
                return ValidationProblem(ModelState);
            }

            return Ok();
        }

    }

}


