using GostControl.AppModels;
using GostControl.AppRepositories;
using GostControl.AppWindows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace GostControl.AppViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ClientRepository _clientRepository;
        private ObservableCollection<Client> _clients;

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set
            {
                _clients = value;
                OnPropertyChanged(nameof(Clients));
            }
        }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
            }
        }

        public ICommand LoadClientsCommand { get; }
        public ICommand AddClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainViewModel()
        {
            _clientRepository = new ClientRepository();
            Clients = new ObservableCollection<Client>();

            LoadClientsCommand = new RelayCommand(LoadClients);
            AddClientCommand = new RelayCommand(AddClient);
            DeleteClientCommand = new RelayCommand(DeleteClient, CanDeleteClient);
            RefreshCommand = new RelayCommand(RefreshData);

            LoadClients(null);
        }

        private void LoadClients(object parameter)
        {
            var clientList = _clientRepository.GetAllClients();
            Clients.Clear();
            foreach (var client in clientList)
            {
                Clients.Add(client);
            }
        }

        private void AddClient(object parameter)
        {
            // Создаем нового клиента
            var newClient = new Client
            {
                ClientID = 0,
                LastName = "Новый",
                FirstName = "Клиент",
                MiddleName = "",
                PhoneNumber = "+7",
                Email = "email@example.com",
                DateOfBirth = System.DateTime.Now.AddYears(-30),
                RegistrationDate = System.DateTime.Now
            };

            var addWindow = new AddEditClientWindow(newClient, true);
            if (addWindow.ShowDialog() == true)
            {
                _clientRepository.AddClient(newClient);
                LoadClients(null);
            }
        }

        private void DeleteClient(object parameter)
        {
            if (SelectedClient != null)
            {
                var result = System.Windows.MessageBox.Show(
                    $"Удалить клиента {SelectedClient.FullName}?",
                    "Подтверждение удаления",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    _clientRepository.DeleteClient(SelectedClient.ClientID);
                    LoadClients(null);
                }
            }
        }

        private void RefreshData(object parameter)
        {
            LoadClients(null);
        }

        private bool CanDeleteClient(object parameter)
        {
            return SelectedClient != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}