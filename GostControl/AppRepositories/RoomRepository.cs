using GostControl.AppModels;
using GostControl.AppServices;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GostControl.AppRepositories
{
    public class RoomRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public RoomRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public List<Room> GetAllRooms()
        {
            var rooms = new List<Room>();

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT r.RoomID, r.RoomNumber, r.CategoryID, r.Floor, 
                           r.HasBalcony, r.IsAvailable,
                           rc.CategoryName, rc.BasePrice, rc.MaxCapacity
                    FROM Hotel.Rooms r
                    INNER JOIN Hotel.RoomCategories rc ON r.CategoryID = rc.CategoryID
                    ORDER BY r.Floor, r.RoomNumber";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                            RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
                            CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                            Floor = reader.GetInt32(reader.GetOrdinal("Floor")),
                            HasBalcony = reader.GetBoolean(reader.GetOrdinal("HasBalcony")),
                            IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                            Category = new RoomCategory
                            {
                                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                BasePrice = reader.GetDecimal(reader.GetOrdinal("BasePrice")),
                                MaxCapacity = reader.GetInt32(reader.GetOrdinal("MaxCapacity"))
                            }
                        });
                    }
                }
            }

            return rooms;
        }

        public List<Room> GetAvailableRooms()
        {
            var rooms = new List<Room>();

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT r.RoomID, r.RoomNumber, r.CategoryID, r.Floor, 
                           r.HasBalcony, r.IsAvailable,
                           rc.CategoryName, rc.BasePrice, rc.MaxCapacity
                    FROM Hotel.Rooms r
                    INNER JOIN Hotel.RoomCategories rc ON r.CategoryID = rc.CategoryID
                    WHERE r.IsAvailable = 1
                    ORDER BY r.Floor, r.RoomNumber";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                            RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
                            CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                            Floor = reader.GetInt32(reader.GetOrdinal("Floor")),
                            HasBalcony = reader.GetBoolean(reader.GetOrdinal("HasBalcony")),
                            IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                            Category = new RoomCategory
                            {
                                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                BasePrice = reader.GetDecimal(reader.GetOrdinal("BasePrice")),
                                MaxCapacity = reader.GetInt32(reader.GetOrdinal("MaxCapacity"))
                            }
                        });
                    }
                }
            }

            return rooms;
        }

        public Room GetRoomById(int roomId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT r.*, rc.CategoryName, rc.BasePrice, rc.MaxCapacity
                    FROM Hotel.Rooms r
                    INNER JOIN Hotel.RoomCategories rc ON r.CategoryID = rc.CategoryID
                    WHERE r.RoomID = @RoomID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Room
                            {
                                RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                                RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber")),
                                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                Floor = reader.GetInt32(reader.GetOrdinal("Floor")),
                                HasBalcony = reader.GetBoolean(reader.GetOrdinal("HasBalcony")),
                                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                                Category = new RoomCategory
                                {
                                    CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                    BasePrice = reader.GetDecimal(reader.GetOrdinal("BasePrice")),
                                    MaxCapacity = reader.GetInt32(reader.GetOrdinal("MaxCapacity"))
                                }
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public int AddRoom(Room room)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Hotel.Rooms 
                    (RoomNumber, CategoryID, Floor, HasBalcony, IsAvailable)
                    OUTPUT INSERTED.RoomID
                    VALUES (@RoomNumber, @CategoryID, @Floor, @HasBalcony, @IsAvailable)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    command.Parameters.AddWithValue("@CategoryID", room.CategoryID);
                    command.Parameters.AddWithValue("@Floor", room.Floor);
                    command.Parameters.AddWithValue("@HasBalcony", room.HasBalcony);
                    command.Parameters.AddWithValue("@IsAvailable", room.IsAvailable);

                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void UpdateRoom(Room room)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Hotel.Rooms 
                    SET RoomNumber = @RoomNumber,
                        CategoryID = @CategoryID,
                        Floor = @Floor,
                        HasBalcony = @HasBalcony,
                        IsAvailable = @IsAvailable
                    WHERE RoomID = @RoomID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", room.RoomID);
                    command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                    command.Parameters.AddWithValue("@CategoryID", room.CategoryID);
                    command.Parameters.AddWithValue("@Floor", room.Floor);
                    command.Parameters.AddWithValue("@HasBalcony", room.HasBalcony);
                    command.Parameters.AddWithValue("@IsAvailable", room.IsAvailable);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteRoom(int roomId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Hotel.Rooms WHERE RoomID = @RoomID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRoomAvailability(int roomId, bool isAvailable)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Hotel.Rooms SET IsAvailable = @IsAvailable WHERE RoomID = @RoomID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomId);
                    command.Parameters.AddWithValue("@IsAvailable", isAvailable);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}