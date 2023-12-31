﻿using System.ComponentModel.DataAnnotations;

namespace CodePulse.API.Models.DTO
{
    public class RegisterRequestDto
    {
        [Required]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Invalid email Adress")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
