using System.ComponentModel.DataAnnotations.Schema;

namespace GostControl.AppModels
{
    [Table("Rooms")]
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public int CategoryID { get; set; }
        public int Floor { get; set; }
        public bool HasBalcony { get; set; }
        public bool IsAvailable { get; set; }

        [ForeignKey("CategoryID")]
        public virtual RoomCategory Category { get; set; }
    }
}