namespace StreamInSync.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        public User()
        {
            Rooms = new List<Room>();
        }

        public int Id { get; set; }

        [Index(IsUnique = true)]
        [Column(TypeName = "VARCHAR")]
        [MaxLength(30)]
        public string Username { get; set; }
     
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public IList<Room> Rooms { get; set; }
    }
}