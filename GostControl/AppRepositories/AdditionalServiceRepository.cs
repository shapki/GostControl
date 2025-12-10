using GostControl.AppModels;
using GostControl.AppServices;
using System.Collections.Generic;
using System.Linq;

namespace GostControl.AppRepositories
{
    public class AdditionalServiceRepository
    {
        private readonly LocalDataService _dataService;

        public AdditionalServiceRepository()
        {
            _dataService = LocalDataService.Instance;
        }

        public List<AdditionalService> GetAllServices()
        {
            return _dataService.AdditionalServices.Where(s => s.IsActive).ToList();
        }

        public List<AdditionalService> GetAllServicesIncludingInactive()
        {
            return _dataService.AdditionalServices.ToList();
        }

        public AdditionalService GetServiceById(int serviceId)
        {
            return _dataService.GetServiceById(serviceId);
        }

        public int AddService(AdditionalService service)
        {
            service.ServiceID = _dataService.GetNextServiceId();
            service.IsActive = true;
            _dataService.AdditionalServices.Add(service);
            return service.ServiceID;
        }

        public void UpdateService(AdditionalService service)
        {
            var existingService = _dataService.GetServiceById(service.ServiceID);
            if (existingService != null)
            {
                int index = _dataService.AdditionalServices.IndexOf(existingService);
                _dataService.AdditionalServices[index] = service;
            }
        }

        public void DeleteService(int serviceId)
        {
            var service = _dataService.GetServiceById(serviceId);
            if (service != null)
            {
                service.IsActive = false;
            }
        }

        public void ActivateService(int serviceId)
        {
            var service = _dataService.GetServiceById(serviceId);
            if (service != null)
            {
                service.IsActive = true;
            }
        }

        public AdditionalService GetServiceByName(string serviceName)
        {
            return _dataService.AdditionalServices
                .FirstOrDefault(s => s.ServiceName.ToLower() == serviceName.ToLower());
        }

        public List<AdditionalService> SearchServices(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllServices();

            return _dataService.AdditionalServices
                .Where(s => s.ServiceName.ToLower().Contains(searchText.ToLower()) ||
                           (s.Description != null && s.Description.ToLower().Contains(searchText.ToLower())))
                .Where(s => s.IsActive)
                .ToList();
        }
    }
}