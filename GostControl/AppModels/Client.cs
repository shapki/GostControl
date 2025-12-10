using System;

namespace GostControl.AppModels
{
    public class Client
    {
        public int ClientID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}";
    }
}