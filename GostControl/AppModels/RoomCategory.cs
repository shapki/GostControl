using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GostControl.AppModels
{
    [Table("RoomCategories")]
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