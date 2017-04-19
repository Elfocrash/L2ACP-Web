using System.ComponentModel.DataAnnotations;

namespace L2ACP.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        public string Test { get; set; } = "true";
    }
}