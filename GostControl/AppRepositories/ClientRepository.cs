using GostControl.AppModels;
using GostControl.AppServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GostControl.AppRepositories
{
    public class ClientRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public ClientRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT ClientID, LastName, FirstName, MiddleName, 
                           PassportSeries, PassportNumber, PhoneNumber, 
                           Email, DateOfBirth, RegistrationDate 
                    FROM Hotel.Clients 
                    ORDER BY LastName, FirstName";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clients.Add(new Client
                        {
                            ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                            PassportSeries = reader.IsDBNull(reader.GetOrdinal("PassportSeries")) ? null : reader.GetString(reader.GetOrdinal("PassportSeries")),
                            PassportNumber = reader.IsDBNull(reader.GetOrdinal("PassportNumber")) ? null : reader.GetString(reader.GetOrdinal("PassportNumber")),
                            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                            DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                            RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate"))
                        });
                    }
                }
            }

            return clients;
        }

        public Client GetClientById(int clientId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Hotel.Clients WHERE ClientID = @ClientID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Client
                            {
                                ClientID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                                PassportSeries = reader.IsDBNull(reader.GetOrdinal("PassportSeries")) ? null : reader.GetString(reader.GetOrdinal("PassportSeries")),
                                PassportNumber = reader.IsDBNull(reader.GetOrdinal("PassportNumber")) ? null : reader.GetString(reader.GetOrdinal("PassportNumber")),
                                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate"))
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public int AddClient(Client client)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Hotel.Clients 
                    (LastName, FirstName, MiddleName, PassportSeries, PassportNumber, 
                     PhoneNumber, Email, DateOfBirth, RegistrationDate)
                    OUTPUT INSERTED.ClientID
                    VALUES (@LastName, @FirstName, @MiddleName, @PassportSeries, @PassportNumber,
                            @PhoneNumber, @Email, @DateOfBirth, GETDATE())";

                using (var command = new SqlCommand(query, connection))
                {
                    AddClientParameters(command, client);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void UpdateClient(Client client)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Hotel.Clients 
                    SET LastName = @LastName,
                        FirstName = @FirstName,
                        MiddleName = @MiddleName,
                        PassportSeries = @PassportSeries,
                        PassportNumber = @PassportNumber,
                        PhoneNumber = @PhoneNumber,
                        Email = @Email,
                        DateOfBirth = @DateOfBirth
                    WHERE ClientID = @ClientID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", client.ClientID);
                    AddClientParameters(command, client);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteClient(int clientId)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Hotel.Clients WHERE ClientID = @ClientID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientId);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddClientParameters(SqlCommand command, Client client)
        {
            command.Parameters.AddWithValue("@LastName", client.LastName);
            command.Parameters.AddWithValue("@FirstName", client.FirstName);
            command.Parameters.Add("@MiddleName", SqlDbType.NVarChar).Value =
                (object)client.MiddleName ?? DBNull.Value;
            command.Parameters.Add("@PassportSeries", SqlDbType.NVarChar, 4).Value =
                (object)client.PassportSeries ?? DBNull.Value;
            command.Parameters.Add("@PassportNumber", SqlDbType.NVarChar, 6).Value =
                (object)client.PassportNumber ?? DBNull.Value;
            command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value =
                (object)client.PhoneNumber ?? DBNull.Value;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value =
                (object)client.Email ?? DBNull.Value;
            command.Parameters.Add("@DateOfBirth", SqlDbType.Date).Value =
                (object)client.DateOfBirth ?? DBNull.Value;
        }
    }
}