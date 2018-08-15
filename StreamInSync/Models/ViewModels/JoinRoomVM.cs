using System.ComponentModel.DataAnnotations;

namespace StreamInSync.Models
{
    public class JoinRoomVM
    {
        [Required(ErrorMessage = "Please provide an inviteCode", AllowEmptyStrings = false)]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Invite code must be 8 alphanumeric characters")]
        public string InviteCode { get; set; }

        [StringLength(50, ErrorMessage = "Password must be less than 50 characters")]
        public string Password { get; set; }

        public bool Success { get; set; }
    }
}