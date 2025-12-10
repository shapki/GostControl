using GostControl.AppModels;
using GostControl.AppServices;
using System.Collections.Generic;
using System.Linq;

namespace GostControl.AppRepositories
{
    public class RoomRepository
    {
        private readonly LocalDataService _dataService;

        public RoomRepository()
        {
            _dataService = LocalDataService.Instance;
        }

        public List<Room> GetAllRooms()
        {
            return _dataService.Rooms.ToList();
        }

        public List<Room> GetAvailableRooms()
        {
            return _dataService.GetAvailableRooms();
        }

        public Room GetRoomById(int roomId)
        {
            return _dataService.GetRoomById(roomId);
        }

        public int AddRoom(Room room)
        {
            room.RoomID = _dataService.GetNextRoomId();

            if (room.CategoryID > 0)
            {
                room.Category = _dataService.GetCategoryById(room.CategoryID);
            }

            _dataService.Rooms.Add(room);
            return room.RoomID;
        }

        public void UpdateRoom(Room room)
        {
            var existingRoom = _dataService.GetRoomById(room.RoomID);
            if (existingRoom != null)
            {
                if (room.CategoryID > 0 && (room.Category == null || room.Category.CategoryID != room.CategoryID))
                {
                    room.Category = _dataService.GetCategoryById(room.CategoryID);
                }

                int index = _dataService.Rooms.IndexOf(existingRoom);
                _dataService.Rooms[index] = room;
            }
        }

        public void DeleteRoom(int roomId)
        {
            var room = _dataService.GetRoomById(roomId);
            if (room != null)
            {
                _dataService.Rooms.Remove(room);
            }
        }

        public void UpdateRoomAvailability(int roomId, bool isAvailable)
        {
            var room = _dataService.GetRoomById(roomId);
            if (room != null)
            {
                room.IsAvailable = isAvailable;
            }
        }

        public List<Room> GetRoomsByCategory(int categoryId)
        {
            return _dataService.Rooms.Where(r => r.CategoryID == categoryId).ToList();
        }

        public List<Room> SearchRooms(string roomNumber)
        {
            if (string.IsNullOrWhiteSpace(roomNumber))
                return GetAllRooms();

            return _dataService.Rooms
                .Where(r => r.RoomNumber.ToLower().Contains(roomNumber.ToLower()))
                .ToList();
        }
    }
}