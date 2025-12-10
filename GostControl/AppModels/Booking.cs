using System.Collections.Generic;
using System;

namespace GostControl.AppModels
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal? TotalCost { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        public virtual Client Client { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<BookingService> BookingServices { get; set; }
    }
}