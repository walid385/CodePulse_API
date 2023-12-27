using System.ComponentModel.DataAnnotations;

namespace CodePulse.API.Models.DTO
{
    public class ForgotPasswordDto
    {
            [Required]
            public string? Email { get; set; }

            [Required]
            public string? ClientURI { get; set; }
   
    }
}
