using System.Collections.Generic;

namespace GostControl.AppModels
{
    public class RoomCategory
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int MaxCapacity { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}