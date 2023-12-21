using System.ComponentModel.DataAnnotations;

namespace CodePulse.API.Models.DTO
{
    public class ConfirmEmailDto
    {
        [Required] 
        public string Token { get; set; }
        [Required]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Invalid email Adress")]
        public string Email { get; set; }
    }
}
