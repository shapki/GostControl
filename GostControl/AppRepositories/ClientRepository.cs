using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GostControl.AppModels;
using GostControl.AppData;

namespace GostControl.AppRepositories
{
    public interface IClientRepository : IDisposable
    {
        List<Client> GetAllClients();
        Task<List<Client>> GetAllClientsAsync();
        Client GetClientById(int clientId);
        Client GetClientWithBookings(int clientId);
        void AddClient(Client client);
        void UpdateClient(Client client);
        void DeleteClient(int clientId);
        List<Client> SearchClients(string searchTerm);
        int GetClientsCount();
    }

    public class ClientRepository : IClientRepository
    {
        private readonly HotelDbContext _context;
        private bool _disposed = false;

        public ClientRepository()
        {
            _context = new HotelDbContext();
        }

        public ClientRepository(HotelDbContext context)
        {
            _context = context;
        }

        public List<Client> GetAllClients()
        {
            try
            {
                return _context.Clients
                    .AsNoTracking()
                    .OrderByDescending(c => c.RegistrationDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при получении списка клиентов: {ex.Message}", ex);
            }
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            try
            {
                return await _context.Clients
                    .AsNoTracking()
                    .OrderByDescending(c => c.RegistrationDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при асинхронном получении списка клиентов: {ex.Message}", ex);
            }
        }

        public Client GetClientById(int clientId)
        {
            try
            {
                return _context.Clients
                    .AsNoTracking()
                    .FirstOrDefault(c => c.ClientID == clientId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при получении клиента по ID: {ex.Message}", ex);
            }
        }

        public Client GetClientWithBookings(int clientId)
        {
            try
            {
                return _context.Clients
                    .Include(c => c.Bookings)
                    .ThenInclude(b => b.Room) // Загрузка информации о номере
                    .AsNoTracking()
                    .FirstOrDefault(c => c.ClientID == clientId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при получении клиента с бронированиями: {ex.Message}", ex);
            }
        }

        public void AddClient(Client client)
        {
            try
            {
                _context.Clients.Add(client);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException($"Ошибка при добавлении клиента в БД: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при добавлении клиента: {ex.Message}", ex);
            }
        }

        public void UpdateClient(Client client)
        {
            try
            {
                var existingClient = _context.Clients.Find(client.ClientID);
                if (existingClient != null)
                {
                    // Обновление только измененных свойств
                    _context.Entry(existingClient).CurrentValues.SetValues(client);
                    _context.SaveChanges();
                }
                else
                {
                    throw new KeyNotFoundException($"Клиент с ID {client.ClientID} не найден");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ApplicationException($"Конфликт параллельного обновления: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при обновлении клиента: {ex.Message}", ex);
            }
        }

        public void DeleteClient(int clientId)
        {
            try
            {
                var client = _context.Clients.Find(clientId);
                if (client != null)
                {
                    // Есть ли связанные бронирования
                    var hasBookings = _context.Bookings.Any(b => b.ClientID == clientId);

                    if (hasBookings)
                    {
                        throw new InvalidOperationException("Невозможно удалить клиента с существующими бронированиями");
                    }

                    _context.Clients.Remove(client);
                    _context.SaveChanges();
                }
                else
                {
                    throw new KeyNotFoundException($"Клиент с ID {clientId} не найден");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при удалении клиента: {ex.Message}", ex);
            }
        }

        public List<Client> SearchClients(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return GetAllClients();

                return _context.Clients
                    .AsNoTracking()
                    .Where(c => c.LastName.Contains(searchTerm) ||
                               c.FirstName.Contains(searchTerm) ||
                               c.PhoneNumber.Contains(searchTerm) ||
                               (c.Email != null && c.Email.Contains(searchTerm)))
                    .OrderByDescending(c => c.RegistrationDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при поиске клиентов: {ex.Message}", ex);
            }
        }

        public int GetClientsCount()
        {
            try
            {
                return _context.Clients.Count();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка при получении количества клиентов: {ex.Message}", ex);
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