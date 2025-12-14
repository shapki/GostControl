using GostControl.AppData;
using GostControl.AppModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GostControl.AppRepositories
{
    public interface IRoomRepository : IDisposable
    {
        List<Room> GetAllRooms();
        List<Room> GetAvailableRooms();
        Room GetRoomById(int roomId);
        Room GetRoomWithCategory(int roomId);
        int AddRoom(Room room);
        void UpdateRoom(Room room);
        void DeleteRoom(int roomId);
        void UpdateRoomAvailability(int roomId, bool isAvailable);
        List<Room> GetRoomsByCategory(int categoryId);
        List<Room> SearchRooms(string roomNumber);
        List<Room> GetAvailableRoomsForDates(DateTime checkIn, DateTime checkOut);
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly HotelDbContext _context;
        private bool _disposed = false;

        public RoomRepository()
        {
            _context = new HotelDbContext();
        }

        public RoomRepository(HotelDbContext context)
        {
            _context = context;
        }

        public List<Room> GetAllRooms()
        {
            return _context.Rooms
                .Include(r => r.Category)
                .AsNoTracking()
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        public List<Room> GetAvailableRooms()
        {
            return _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.IsAvailable)
                .AsNoTracking()
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        public Room GetRoomById(int roomId)
        {
            return _context.Rooms
                .Include(r => r.Category)
                .AsNoTracking()
                .FirstOrDefault(r => r.RoomID == roomId);
        }

        public Room GetRoomWithCategory(int roomId)
        {
            return _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefault(r => r.RoomID == roomId);
        }

        public int AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return room.RoomID;
        }

        public void UpdateRoom(Room room)
        {
            _context.Entry(room).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteRoom(int roomId)
        {
            var room = _context.Rooms.Find(roomId);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
        }

        public void UpdateRoomAvailability(int roomId, bool isAvailable)
        {
            var room = _context.Rooms.Find(roomId);
            if (room != null)
            {
                room.IsAvailable = isAvailable;
                _context.SaveChanges();
            }
        }

        public List<Room> GetRoomsByCategory(int categoryId)
        {
            return _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.CategoryID == categoryId)
                .AsNoTracking()
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        public List<Room> SearchRooms(string roomNumber)
        {
            if (string.IsNullOrWhiteSpace(roomNumber))
                return GetAllRooms();

            return _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.RoomNumber.ToLower().Contains(roomNumber.ToLower()))
                .AsNoTracking()
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        public List<Room> GetAvailableRoomsForDates(DateTime checkIn, DateTime checkOut)
        {
            var bookedRoomIds = _context.Bookings
                .Where(b => b.Status == "Подтверждено" &&
                           b.CheckInDate < checkOut &&
                           b.CheckOutDate > checkIn)
                .Select(b => b.RoomID)
                .ToList();

            return _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.IsAvailable && !bookedRoomIds.Contains(r.RoomID))
                .AsNoTracking()
                .OrderBy(r => r.RoomNumber)
                .ToList();
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