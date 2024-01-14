using System.ComponentModel.DataAnnotations;

namespace CodePulse.API.Models.DTO
{
    public class ContactUsDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Invalid email Adress")]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
