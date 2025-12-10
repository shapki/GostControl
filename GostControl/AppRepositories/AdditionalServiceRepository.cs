using GostControl.AppModels;
using GostControl.AppServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GostControl.AppRepositories
{
    public class AdditionalServiceRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public AdditionalServiceRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public List<AdditionalService> GetAllServices()
        {
            var services = new List<AdditionalService>();

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Hotel.AdditionalServices WHERE IsActive = 1 ORDER BY ServiceName";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        services.Add(new AdditionalService
                        {
                            ServiceID = reader.GetInt32(reader.GetOrdinal("ServiceID")),
                            ServiceName = reader.GetString(reader.GetOrdinal("ServiceName")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        });
                    }
                }
            }

            return services;
        }

        public AdditionalService GetServiceById(int serviceId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Hotel.AdditionalServices WHERE ServiceID = @ServiceID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceID", serviceId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AdditionalService
                            {
                                ServiceID = reader.GetInt32(reader.GetOrdinal("ServiceID")),
                                ServiceName = reader.GetString(reader.GetOrdinal("ServiceName")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public int AddService(AdditionalService service)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Hotel.AdditionalServices 
                    (ServiceName, Description, Price, IsActive)
                    OUTPUT INSERTED.ServiceID
                    VALUES (@ServiceName, @Description, @Price, @IsActive)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                    command.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value =
                        (object)service.Description ?? DBNull.Value;
                    command.Parameters.AddWithValue("@Price", service.Price);
                    command.Parameters.AddWithValue("@IsActive", service.IsActive);

                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void UpdateService(AdditionalService service)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Hotel.AdditionalServices 
                    SET ServiceName = @ServiceName,
                        Description = @Description,
                        Price = @Price,
                        IsActive = @IsActive
                    WHERE ServiceID = @ServiceID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceID", service.ServiceID);
                    command.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                    command.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value =
                        (object)service.Description ?? DBNull.Value;
                    command.Parameters.AddWithValue("@Price", service.Price);
                    command.Parameters.AddWithValue("@IsActive", service.IsActive);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteService(int serviceId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Hotel.AdditionalServices SET IsActive = 0 WHERE ServiceID = @ServiceID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceID", serviceId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}