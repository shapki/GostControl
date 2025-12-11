using GostControl.AppModels;
using GostControl.AppServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GostControl.AppRepositories
{
    public class BookingRepository
    {
        private readonly LocalDataService _dataService;

        public BookingRepository()
        {
            _dataService = LocalDataService.Instance;
        }

        public List<Booking> GetAllBookings()
        {
            return _dataService.Bookings.ToList();
        }

        public Booking GetBookingById(int bookingId)
        {
            return _dataService.GetBookingById(bookingId);
        }

        public List<Booking> GetBookingsByClient(int clientId)
        {
            if (clientId <= 0) return new List<Booking>();

            return _dataService.Bookings
                .Where(b => b.ClientID == clientId)
                .ToList();
        }

        public List<Booking> GetActiveBookings()
        {
            return _dataService.GetActiveBookings();
        }

        public List<Booking> GetBookingsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dataService.Bookings
                .Where(b => b.CheckInDate <= endDate && b.CheckOutDate >= startDate)
                .ToList();
        }

        public int AddBooking(Booking booking)
        {
            booking.BookingID = _dataService.GetNextBookingId();

            if (booking.ClientID > 0)
            {
                booking.Client = _dataService.GetClientById(booking.ClientID);
            }

            if (booking.RoomID > 0)
            {
                booking.Room = _dataService.GetRoomById(booking.RoomID);

                if (booking.Status == "Подтверждено" && booking.Room != null)
                {
                    booking.Room.IsAvailable = false;
                }
            }

            if (booking.BookingDate == DateTime.MinValue)
            {
                booking.BookingDate = DateTime.Now;
            }

            _dataService.Bookings.Add(booking);
            return booking.BookingID;
        }

        public void UpdateBooking(Booking booking)
        {
            var existingBooking = _dataService.GetBookingById(booking.BookingID);
            if (existingBooking != null)
            {
                if (booking.ClientID > 0 && booking.Client == null)
                {
                    booking.Client = _dataService.GetClientById(booking.ClientID);
                }

                if (booking.RoomID > 0 && booking.Room == null)
                {
                    booking.Room = _dataService.GetRoomById(booking.RoomID);
                }

                if (existingBooking.Status != booking.Status && booking.Room != null)
                {
                    if (booking.Status == "Подтверждено" || booking.Status == "Завершено")
                    {
                        booking.Room.IsAvailable = false;
                    }
                    else if (booking.Status == "Отменено")
                    {
                        booking.Room.IsAvailable = true;
                    }
                }

                int index = _dataService.Bookings.IndexOf(existingBooking);
                _dataService.Bookings[index] = booking;
            }
        }

        public void DeleteBooking(int bookingId)
        {
            var booking = _dataService.GetBookingById(bookingId);
            if (booking != null)
            {
                if (booking.Room != null && booking.Status != "Отменено")
                {
                    booking.Room.IsAvailable = true;
                }

                _dataService.Bookings.Remove(booking);
            }
        }

        public void UpdateBookingStatus(int bookingId, string status)
        {
            var booking = _dataService.GetBookingById(bookingId);
            if (booking != null)
            {
                booking.Status = status;

                if (booking.Room != null)
                {
                    if (status == "Подтверждено" || status == "Завершено")
                    {
                        booking.Room.IsAvailable = false;
                    }
                    else if (status == "Отменено")
                    {
                        booking.Room.IsAvailable = true;
                    }
                }
            }
        }

        public decimal CalculateBookingCost(int bookingId)
        {
            var booking = _dataService.GetBookingById(bookingId);
            if (booking == null)
                return 0;

            if (booking.Room != null && booking.Room.Category != null)
            {
                int days = (booking.CheckOutDate - booking.CheckInDate).Days;
                if (days > 0)
                {
                    return booking.Room.Category.BasePrice * days;
                }
            }

            return 0;
        }
    }
}