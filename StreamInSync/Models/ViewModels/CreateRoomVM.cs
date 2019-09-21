using System;
using System.ComponentModel.DataAnnotations;

namespace StreamInSync.Models
{
    public class CreateRoomVM
    {
        [Required(ErrorMessage = "Please provide a room name", AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Room name must be less than 50 characters.")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "Password must be less than 50 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please provide a room name", AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Room name must be less than 50 characters.")]
        public string ProgrammeName { get; set; }

        [Required(ErrorMessage = "Please provide a programme runtime.")]
        [DataType(DataType.Duration)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan RuntimeInSeconds { get; set; }

        [Required(ErrorMessage = "Please provide a programme start time.")]
        [DataType(DataType.DateTime)]
        public DateTime ProgrammeStartTime { get; set; }

        public bool Success { get; set; }
    }
}