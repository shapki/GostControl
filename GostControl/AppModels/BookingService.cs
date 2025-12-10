using System;

namespace GostControl.AppModels
{
    public class BookingService
    {
        public int BookingServiceID { get; set; }
        public int BookingID { get; set; }
        public int ServiceID { get; set; }
        public int Quantity { get; set; }
        public DateTime? ServiceDate { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual AdditionalService Service { get; set; }
    }
}