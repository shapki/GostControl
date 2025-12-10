using GostControl.AppModels;
using GostControl.AppServices;
using System.Collections.Generic;
using System.Linq;

namespace GostControl.AppRepositories
{
    public class ClientRepository
    {
        private readonly LocalDataService _dataService;

        public ClientRepository()
        {
            _dataService = LocalDataService.Instance;
        }

        public List<Client> GetAllClients()
        {
            return _dataService.Clients.ToList();
        }

        public Client GetClientById(int clientId)
        {
            return _dataService.GetClientById(clientId);
        }

        public int AddClient(Client client)
        {
            client.ClientID = _dataService.GetNextClientId();
            client.RegistrationDate = System.DateTime.Now;
            _dataService.Clients.Add(client);
            return client.ClientID;
        }

        public void UpdateClient(Client client)
        {
            var existingClient = _dataService.GetClientById(client.ClientID);
            if (existingClient != null)
            {
                client.RegistrationDate = existingClient.RegistrationDate;

                int index = _dataService.Clients.IndexOf(existingClient);
                _dataService.Clients[index] = client;
            }
        }

        public void DeleteClient(int clientId)
        {
            var client = _dataService.GetClientById(clientId);
            if (client != null)
            {
                _dataService.Clients.Remove(client);
            }
        }

        public List<Client> SearchClients(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllClients();

            return _dataService.SearchClients(searchText);
        }
    }
}