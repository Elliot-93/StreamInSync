using System.ComponentModel.DataAnnotations;

namespace StreamInSync.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Please provide username", AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Name must be less than 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please provide Password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be 6 characters long.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password does not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public bool Success { get; set; }
    }
}