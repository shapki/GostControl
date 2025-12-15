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
        private readonly IClientRepository _clientRepository;
        private readonly IBookingRepository _bookingRepository;
        private ObservableCollection<Client> _clients;
        private Client _selectedClient;
        private string _searchText;

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set
            {
                _clients = value;
                OnPropertyChanged(nameof(Clients));
            }
        }

        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
                OnPropertyChanged(nameof(ClientFullName));
                OnPropertyChanged(nameof(IsClientSelected));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    PerformSearch();
                }
            }
        }

        // Вычисляемое свойство для полного имени
        public string ClientFullName => SelectedClient != null ?
            $"{SelectedClient.LastName} {SelectedClient.FirstName} {SelectedClient.MiddleName}".Trim() :
            string.Empty;

        public bool IsClientSelected => SelectedClient != null;

        // Команды
        public ICommand LoadClientsCommand { get; }
        public ICommand AddClientCommand { get; }
        public ICommand EditClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public MainViewModel()
        {
            _clientRepository = new ClientRepository();
            _bookingRepository = new BookingRepository();
            Clients = new ObservableCollection<Client>();

            // Инициализация команд
            LoadClientsCommand = new RelayCommand(LoadClients);
            AddClientCommand = new RelayCommand(AddClient);
            EditClientCommand = new RelayCommand(EditClient, CanEditClient);
            DeleteClientCommand = new RelayCommand(DeleteClient, CanDeleteClient);
            RefreshCommand = new RelayCommand(RefreshData);
            SearchCommand = new RelayCommand(PerformSearch);
            ClearSearchCommand = new RelayCommand(ClearSearch);

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

                // Обновление статуса при первом запуске
                if (clientList.Any() && SelectedClient == null)
                {
                    SelectedClient = Clients.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки клиентов: {ex.Message}");
            }
        }

        private void AddClient(object parameter)
        {
            try
            {
                // Создаем нового клиента
                var newClient = new Client();

                // Используем конструктор с параметром bool (true = новый клиент)
                var addWindow = new AddEditClientWindow(newClient, true)
                {
                    Owner = Application.Current.MainWindow
                };

                if (addWindow.ShowDialog() == true)
                {
                    // Клиент уже обновлен в окне, добавляем в репозиторий
                    _clientRepository.AddClient(newClient);

                    LoadClients(null);
                    ShowSuccessMessage("Клиент успешно добавлен!");

                    // Выбор нового клиента
                    SelectedClient = Clients.FirstOrDefault(c => c.ClientID == newClient.ClientID);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при добавлении клиента: {ex.Message}");
            }
        }

        public void EditClient(object parameter)
        {
            try
            {
                if (SelectedClient == null)
                {
                    ShowInfoMessage("Выберите клиента для редактирования");
                    return;
                }

                // Получение актуальных данных клиента из БД
                var clientToEdit = _clientRepository.GetClientById(SelectedClient.ClientID);
                if (clientToEdit == null)
                {
                    ShowErrorMessage("Клиент не найден в базе данных");
                    return;
                }

                // Создаем копию клиента для редактирования
                var clientCopy = new Client
                {
                    ClientID = clientToEdit.ClientID,
                    LastName = clientToEdit.LastName,
                    FirstName = clientToEdit.FirstName,
                    MiddleName = clientToEdit.MiddleName,
                    PhoneNumber = clientToEdit.PhoneNumber,
                    Email = clientToEdit.Email,
                    PassportSeries = clientToEdit.PassportSeries,
                    PassportNumber = clientToEdit.PassportNumber,
                    DateOfBirth = clientToEdit.DateOfBirth,
                    RegistrationDate = clientToEdit.RegistrationDate
                };

                // Используем конструктор с параметром bool (false = редактирование)
                var editWindow = new AddEditClientWindow(clientCopy, false)
                {
                    Owner = Application.Current.MainWindow
                };

                if (editWindow.ShowDialog() == true)
                {
                    // Обновляем оригинальный объект данными из копии
                    clientToEdit.LastName = clientCopy.LastName;
                    clientToEdit.FirstName = clientCopy.FirstName;
                    clientToEdit.MiddleName = clientCopy.MiddleName;
                    clientToEdit.PhoneNumber = clientCopy.PhoneNumber;
                    clientToEdit.Email = clientCopy.Email;
                    clientToEdit.PassportSeries = clientCopy.PassportSeries;
                    clientToEdit.PassportNumber = clientCopy.PassportNumber;
                    clientToEdit.DateOfBirth = clientCopy.DateOfBirth;

                    _clientRepository.UpdateClient(clientToEdit);

                    // Обновление списка
                    LoadClients(null);

                    // Восстановление выбора
                    SelectedClient = Clients.FirstOrDefault(c => c.ClientID == clientToEdit.ClientID);

                    ShowSuccessMessage("Данные клиента успешно обновлены!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при редактировании клиента: {ex.Message}");
            }
        }

        private void DeleteClient(object parameter)
        {
            try
            {
                if (SelectedClient == null)
                {
                    ShowInfoMessage("Выберите клиента для удаления");
                    return;
                }

                // Есть ли бронирования у клиента
                var bookings = _bookingRepository.GetBookingsByClient(SelectedClient.ClientID);
                bool hasBookings = bookings.Any();

                string message = hasBookings ?
                    $"У клиента есть {bookings.Count} бронирований. Вы уверены, что хотите удалить клиента?\n{SelectedClient.LastName} {SelectedClient.FirstName}?" :
                    $"Вы уверены, что хотите удалить клиента:\n{SelectedClient.LastName} {SelectedClient.FirstName}?";

                var result = MessageBox.Show(
                    message,
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    hasBookings ? MessageBoxImage.Warning : MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _clientRepository.DeleteClient(SelectedClient.ClientID);
                    LoadClients(null);

                    ShowSuccessMessage("Клиент успешно удален!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при удалении клиента: {ex.Message}");
            }
        }

        private void RefreshData(object parameter)
        {
            LoadClients(null);
        }

        private void PerformSearch(object parameter = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    LoadClients(null);
                    return;
                }

                var searchResults = _clientRepository.SearchClients(SearchText);
                Clients.Clear();
                foreach (var client in searchResults)
                {
                    Clients.Add(client);
                }

                if (searchResults.Any())
                {
                    SelectedClient = Clients.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при поиске: {ex.Message}");
            }
        }

        private void ClearSearch(object parameter = null)
        {
            SearchText = string.Empty;
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

        // Вспомогательные методы для сообщений
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}