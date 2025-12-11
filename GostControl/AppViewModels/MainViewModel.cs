using GostControl.AppModels;
using GostControl.AppRepositories;
using GostControl.AppWindows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
        public ICommand EditClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainViewModel()
        {
            _clientRepository = new ClientRepository();
            Clients = new ObservableCollection<Client>();

            LoadClientsCommand = new RelayCommand(LoadClients);
            AddClientCommand = new RelayCommand(AddClient);
            EditClientCommand = new RelayCommand(EditClient, CanEditClient);
            DeleteClientCommand = new RelayCommand(DeleteClient, CanDeleteClient);
            RefreshCommand = new RelayCommand(RefreshData);

            LoadClients(null);
        }

        private void LoadClients(object parameter)
        {
            try
            {
                var clientList = _clientRepository.GetAllClients();
                Clients.Clear();
                foreach (var client in clientList)
                {
                    Clients.Add(client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddClient(object parameter)
        {
            try
            {
                var newClient = new Client
                {
                    ClientID = 0,
                    LastName = "",
                    FirstName = "",
                    MiddleName = "",
                    PassportSeries = "",
                    PassportNumber = "",
                    PhoneNumber = "",
                    Email = "",
                    DateOfBirth = null,
                    RegistrationDate = DateTime.Now
                };

                var editWindow = new EditClientWindow(newClient)
                {
                    Title = "Добавление нового клиента"
                };

                if (editWindow.ShowDialog() == true)
                {
                    _clientRepository.AddClient(newClient);
                    LoadClients(null);

                    MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void EditClient(object parameter)
        {
            try
            {
                if (SelectedClient == null)
                {
                    MessageBox.Show("Выберите клиента для редактирования", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var editWindow = new EditClientWindow(SelectedClient);

                if (editWindow.ShowDialog() == true)
                {
                    LoadClients(null);

                    SelectedClient = Clients.FirstOrDefault(c => c.ClientID == SelectedClient.ClientID);

                    MessageBox.Show("Данные клиента успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteClient(object parameter)
        {
            try
            {
                if (SelectedClient == null)
                {
                    MessageBox.Show("Выберите клиента для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить клиента:\n{SelectedClient.FullName}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _clientRepository.DeleteClient(SelectedClient.ClientID);
                    LoadClients(null);

                    MessageBox.Show("Клиент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshData(object parameter)
        {
            LoadClients(null);
        }

        private bool CanEditClient(object parameter)
        {
            return SelectedClient != null;
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