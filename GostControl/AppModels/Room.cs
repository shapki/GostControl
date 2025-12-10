namespace GostControl.AppModels
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public int CategoryID { get; set; }
        public int Floor { get; set; }
        public bool HasBalcony { get; set; }
        public bool IsAvailable { get; set; }

        public virtual RoomCategory Category { get; set; }
    }
}