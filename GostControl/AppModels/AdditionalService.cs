using System.Collections.Generic;

namespace GostControl.AppModels
{
    public class AdditionalService
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<BookingService> BookingServices { get; set; }
    }
}