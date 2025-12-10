using GostControl.AppModels;
using GostControl.AppServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GostControl.AppRepositories
{
    public class BookingRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public BookingRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public List<Booking> GetAllBookings()
        {
            var bookings = new List<Booking>();

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT b.BookingID, b.ClientID, b.RoomID, b.BookingDate, 
                           b.CheckInDate, b.CheckOutDate, b.TotalCost, b.Status, b.Notes,
                           c.LastName, c.FirstName, c.MiddleName,
                           r.RoomNumber, rc.CategoryName
                    FROM Hotel.Bookings b
                    INNER JOIN Hotel.Clients c ON b.ClientID = c.ClientID
                    INNER JOIN Hotel.Rooms r ON b.RoomID = r.RoomID
                    INNER JOIN Hotel.RoomCategories rc ON r.CategoryID = rc.CategoryID
                    ORDER BY b.BookingDate DESC";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bookings.Add(new Booking
                        {
                            BookingID = reader.GetInt32(reader.GetOrdinal("BookingID")),
                            ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                            RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                            BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                            CheckInDate = reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
                            CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CheckOutDate")),
                            TotalCost = reader.IsDBNull(reader.GetOrdinal("TotalCost")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("TotalCost")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                            Client = new Client
                            {
                                ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName"))
                            },
                            Room = new Room
                            {
                                RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                                RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber"))
                            }
                        });
                    }
                }
            }

            return bookings;
        }

        public Booking GetBookingById(int bookingId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT b.*, c.LastName, c.FirstName, c.MiddleName,
                           r.RoomNumber, rc.CategoryName
                    FROM Hotel.Bookings b
                    INNER JOIN Hotel.Clients c ON b.ClientID = c.ClientID
                    INNER JOIN Hotel.Rooms r ON b.RoomID = r.RoomID
                    INNER JOIN Hotel.RoomCategories rc ON r.CategoryID = rc.CategoryID
                    WHERE b.BookingID = @BookingID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookingID", bookingId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Booking
                            {
                                BookingID = reader.GetInt32(reader.GetOrdinal("BookingID")),
                                ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                                RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                                BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                                CheckInDate = reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
                                CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CheckOutDate")),
                                TotalCost = reader.IsDBNull(reader.GetOrdinal("TotalCost")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("TotalCost")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                Client = new Client
                                {
                                    ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName"))
                                },
                                Room = new Room
                                {
                                    RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                                    RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber"))
                                }
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public List<Booking> GetBookingsByClient(int clientId)
        {
            var bookings = new List<Booking>();

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT b.*, r.RoomNumber
                    FROM Hotel.Bookings b
                    INNER JOIN Hotel.Rooms r ON b.RoomID = r.RoomID
                    WHERE b.ClientID = @ClientID
                    ORDER BY b.CheckInDate DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bookings.Add(new Booking
                            {
                                BookingID = reader.GetInt32(reader.GetOrdinal("BookingID")),
                                ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                                RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                                BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                                CheckInDate = reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
                                CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CheckOutDate")),
                                TotalCost = reader.IsDBNull(reader.GetOrdinal("TotalCost")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("TotalCost")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                Room = new Room
                                {
                                    RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                                    RoomNumber = reader.GetString(reader.GetOrdinal("RoomNumber"))
                                }
                            });
                        }
                    }
                }
            }

            return bookings;
        }

        public int AddBooking(Booking booking)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Hotel.Bookings 
                    (ClientID, RoomID, BookingDate, CheckInDate, CheckOutDate, 
                     TotalCost, Status, Notes)
                    OUTPUT INSERTED.BookingID
                    VALUES (@ClientID, @RoomID, @BookingDate, @CheckInDate, @CheckOutDate,
                            @TotalCost, @Status, @Notes)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", booking.ClientID);
                    command.Parameters.AddWithValue("@RoomID", booking.RoomID);
                    command.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                    command.Parameters.AddWithValue("@CheckInDate", booking.CheckInDate);
                    command.Parameters.AddWithValue("@CheckOutDate", booking.CheckOutDate);
                    command.Parameters.Add("@TotalCost", SqlDbType.Decimal).Value =
                        (object)booking.TotalCost ?? DBNull.Value;
                    command.Parameters.AddWithValue("@Status", booking.Status);
                    command.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value =
                        (object)booking.Notes ?? DBNull.Value;

                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void UpdateBooking(Booking booking)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Hotel.Bookings 
                    SET ClientID = @ClientID,
                        RoomID = @RoomID,
                        BookingDate = @BookingDate,
                        CheckInDate = @CheckInDate,
                        CheckOutDate = @CheckOutDate,
                        TotalCost = @TotalCost,
                        Status = @Status,
                        Notes = @Notes
                    WHERE BookingID = @BookingID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookingID", booking.BookingID);
                    command.Parameters.AddWithValue("@ClientID", booking.ClientID);
                    command.Parameters.AddWithValue("@RoomID", booking.RoomID);
                    command.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                    command.Parameters.AddWithValue("@CheckInDate", booking.CheckInDate);
                    command.Parameters.AddWithValue("@CheckOutDate", booking.CheckOutDate);
                    command.Parameters.Add("@TotalCost", SqlDbType.Decimal).Value =
                        (object)booking.TotalCost ?? DBNull.Value;
                    command.Parameters.AddWithValue("@Status", booking.Status);
                    command.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value =
                        (object)booking.Notes ?? DBNull.Value;

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBooking(int bookingId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Hotel.Bookings WHERE BookingID = @BookingID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookingID", bookingId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateBookingStatus(int bookingId, string status)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Hotel.Bookings SET Status = @Status WHERE BookingID = @BookingID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookingID", bookingId);
                    command.Parameters.AddWithValue("@Status", status);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}