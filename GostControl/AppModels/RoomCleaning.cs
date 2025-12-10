using System;

namespace GostControl.AppModels
{
    public class RoomCleaning
    {
        public int CleaningID { get; set; }
        public int RoomID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime CleaningDate { get; set; }
        public TimeSpan? CleaningTime { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        public virtual Room Room { get; set; }
        public virtual Employee Employee { get; set; }
    }
}