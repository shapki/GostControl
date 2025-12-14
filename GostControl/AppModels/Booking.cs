using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GostControl.AppModels
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingID { get; set; }

        [Required]
        public int ClientID { get; set; }

        [Required]
        public int RoomID { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalCost { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Подтверждено";

        [MaxLength(500)]
        public string Notes { get; set; }

        [ForeignKey("ClientID")]
        public virtual Client Client { get; set; }

        [ForeignKey("RoomID")]
        public virtual Room Room { get; set; }

        public virtual ICollection<BookingService> BookingServices { get; set; }

        public Booking()
        {
            BookingServices = new HashSet<BookingService>();
        }
    }
}