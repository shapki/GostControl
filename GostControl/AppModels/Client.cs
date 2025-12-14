using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GostControl.AppModels
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientID { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [MaxLength(4)]
        public string PassportSeries { get; set; }

        [MaxLength(6)]
        public string PassportNumber { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public virtual ICollection<Booking> Bookings { get; set; }

        public Client()
        {
            Bookings = new HashSet<Booking>();
        }
    }
}