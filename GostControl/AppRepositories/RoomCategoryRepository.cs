using GostControl.AppModels;
using GostControl.AppServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GostControl.AppRepositories
{
    public class RoomCategoryRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public RoomCategoryRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public List<RoomCategory> GetAllCategories()
        {
            var categories = new List<RoomCategory>();

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Hotel.RoomCategories ORDER BY BasePrice";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new RoomCategory
                        {
                            CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                            CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            BasePrice = reader.GetDecimal(reader.GetOrdinal("BasePrice")),
                            MaxCapacity = reader.GetInt32(reader.GetOrdinal("MaxCapacity"))
                        });
                    }
                }
            }

            return categories;
        }

        public RoomCategory GetCategoryById(int categoryId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Hotel.RoomCategories WHERE CategoryID = @CategoryID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", categoryId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RoomCategory
                            {
                                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                BasePrice = reader.GetDecimal(reader.GetOrdinal("BasePrice")),
                                MaxCapacity = reader.GetInt32(reader.GetOrdinal("MaxCapacity"))
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public int AddCategory(RoomCategory category)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Hotel.RoomCategories 
                    (CategoryName, Description, BasePrice, MaxCapacity)
                    OUTPUT INSERTED.CategoryID
                    VALUES (@CategoryName, @Description, @BasePrice, @MaxCapacity)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    command.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value =
                        (object)category.Description ?? DBNull.Value;
                    command.Parameters.AddWithValue("@BasePrice", category.BasePrice);
                    command.Parameters.AddWithValue("@MaxCapacity", category.MaxCapacity);

                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void UpdateCategory(RoomCategory category)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Hotel.RoomCategories 
                    SET CategoryName = @CategoryName,
                        Description = @Description,
                        BasePrice = @BasePrice,
                        MaxCapacity = @MaxCapacity
                    WHERE CategoryID = @CategoryID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                    command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    command.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value =
                        (object)category.Description ?? DBNull.Value;
                    command.Parameters.AddWithValue("@BasePrice", category.BasePrice);
                    command.Parameters.AddWithValue("@MaxCapacity", category.MaxCapacity);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCategory(int categoryId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Hotel.RoomCategories WHERE CategoryID = @CategoryID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", categoryId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}