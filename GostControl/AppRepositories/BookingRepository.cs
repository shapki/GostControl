using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GostControl.AppModels;
using GostControl.AppData;

namespace GostControl.AppRepositories
{
    public interface IBookingRepository : IDisposable
    {
        List<Booking> GetBookingsByClient(int clientId);
        List<Booking> GetActiveBookings();
        int GetBookingsCountByClient(int clientId);
        void AddBooking(Booking booking);
    }

    public class BookingRepository : IBookingRepository
    {
        private readonly HotelDbContext _context;
        private bool _disposed = false;

        public BookingRepository()
        {
            _context = new HotelDbContext();
        }

        public BookingRepository(HotelDbContext context)
        {
            _context = context;
        }

        public List<Booking> GetBookingsByClient(int clientId)
        {
            try
            {
                return _context.Bookings
                    .Include(b => b.Room)
                    .ThenInclude(r => r.Category)
                    .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                    .Where(b => b.ClientID == clientId)
                    .OrderByDescending(b => b.BookingDate)
                    .AsNoTracking()
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при получении бронирований клиента: {ex.Message}", ex);
            }
        }

        public List<Booking> GetActiveBookings()
        {
            try
            {
                var today = DateTime.Today;

                return _context.Bookings
                    .Include(b => b.Client)
                    .Include(b => b.Room)
                    .Where(b => b.Status == "Подтверждено" &&
                               b.CheckInDate <= today &&
                               b.CheckOutDate >= today)
                    .OrderBy(b => b.CheckInDate)
                    .AsNoTracking()
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при получении активных бронирований: {ex.Message}", ex);
            }
        }

        public int GetBookingsCountByClient(int clientId)
        {
            try
            {
                return _context.Bookings.Count(b => b.ClientID == clientId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при подсчете бронирований клиента: {ex.Message}", ex);
            }
        }

        public void AddBooking(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при добавлении бронирования: {ex.Message}", ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}