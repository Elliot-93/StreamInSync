using System.ComponentModel.DataAnnotations;

namespace StreamInSync.Models
{
    public class JoinRoomVM
    {
        [Required(ErrorMessage = "Please provide a room name", AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Room name must be less than 50 characters.")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "Password must be less than 50 characters")]
        public string Password { get; set; }

        public bool Success { get; set; }
    }
}