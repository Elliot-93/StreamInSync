using System.ComponentModel.DataAnnotations;

namespace StreamInSync.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Please provide username", AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please provide Password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Failed { get; set; }
    }
}