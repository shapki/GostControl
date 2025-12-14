using GostControl.AppData;
using GostControl.AppModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GostControl.AppRepositories
{
    public interface IAdditionalServiceRepository : IDisposable
    {
        List<AdditionalService> GetAllServices();
        List<AdditionalService> GetAllServicesIncludingInactive();
        AdditionalService GetServiceById(int serviceId);
        int AddService(AdditionalService service);
        void UpdateService(AdditionalService service);
        void DeleteService(int serviceId);
        void ActivateService(int serviceId);
        AdditionalService GetServiceByName(string serviceName);
        List<AdditionalService> SearchServices(string searchText);
    }

    public class AdditionalServiceRepository : IAdditionalServiceRepository
    {
        private readonly HotelDbContext _context;
        private bool _disposed = false;

        public AdditionalServiceRepository()
        {
            _context = new HotelDbContext();
        }

        public AdditionalServiceRepository(HotelDbContext context)
        {
            _context = context;
        }

        public List<AdditionalService> GetAllServices()
        {
            return _context.AdditionalServices
                .Where(s => s.IsActive)
                .AsNoTracking()
                .OrderBy(s => s.ServiceName)
                .ToList();
        }

        public List<AdditionalService> GetAllServicesIncludingInactive()
        {
            return _context.AdditionalServices
                .AsNoTracking()
                .OrderBy(s => s.ServiceName)
                .ToList();
        }

        public AdditionalService GetServiceById(int serviceId)
        {
            return _context.AdditionalServices
                .AsNoTracking()
                .FirstOrDefault(s => s.ServiceID == serviceId);
        }

        public int AddService(AdditionalService service)
        {
            service.IsActive = true;
            _context.AdditionalServices.Add(service);
            _context.SaveChanges();
            return service.ServiceID;
        }

        public void UpdateService(AdditionalService service)
        {
            _context.Entry(service).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteService(int serviceId)
        {
            var service = _context.AdditionalServices.Find(serviceId);
            if (service != null)
            {
                service.IsActive = false;
                _context.SaveChanges();
            }
        }

        public void ActivateService(int serviceId)
        {
            var service = _context.AdditionalServices.Find(serviceId);
            if (service != null)
            {
                service.IsActive = true;
                _context.SaveChanges();
            }
        }

        public AdditionalService GetServiceByName(string serviceName)
        {
            return _context.AdditionalServices
                .AsNoTracking()
                .FirstOrDefault(s => s.ServiceName.ToLower() == serviceName.ToLower());
        }

        public List<AdditionalService> SearchServices(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllServices();

            return _context.AdditionalServices
                .Where(s => (s.ServiceName.ToLower().Contains(searchText.ToLower()) ||
                           (s.Description != null && s.Description.ToLower().Contains(searchText.ToLower()))) &&
                           s.IsActive)
                .AsNoTracking()
                .OrderBy(s => s.ServiceName)
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