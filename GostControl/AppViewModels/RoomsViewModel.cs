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
    public class RoomsViewModel : INotifyPropertyChanged
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomCategoryRepository _categoryRepository;
        private ObservableCollection<Room> _rooms;
        private Room _selectedRoom;
        private string _searchText;
        public int AvailableRoomsCount => Rooms?.Count(r => r.IsAvailable) ?? 0;
        public int TotalRoomsCount => Rooms?.Count ?? 0;

        public ObservableCollection<Room> Rooms
        {
            get => _rooms;
            set
            {
                _rooms = value;
                OnPropertyChanged(nameof(Rooms));
            }
        }

        public Room SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged(nameof(SelectedRoom));
                OnPropertyChanged(nameof(IsRoomSelected));
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

        public bool IsRoomSelected => SelectedRoom != null;

        // Команды
        public ICommand LoadRoomsCommand { get; }
        public ICommand AddRoomCommand { get; }
        public ICommand EditRoomCommand { get; }
        public ICommand DeleteRoomCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public RoomsViewModel()
        {
            _roomRepository = new RoomRepository();
            _categoryRepository = new RoomCategoryRepository();
            Rooms = new ObservableCollection<Room>();

            // Инициализация команд
            LoadRoomsCommand = new RelayCommand(LoadRooms);
            AddRoomCommand = new RelayCommand(AddRoom);
            EditRoomCommand = new RelayCommand(EditRoom, CanEditRoom);
            DeleteRoomCommand = new RelayCommand(DeleteRoom, CanDeleteRoom);
            RefreshCommand = new RelayCommand(RefreshData);
            SearchCommand = new RelayCommand(PerformSearch);
            ClearSearchCommand = new RelayCommand(ClearSearch);

            LoadRooms(null);
        }

        private void LoadRooms(object parameter)
        {
            try
            {
                var roomList = _roomRepository.GetAllRooms();
                Rooms.Clear();
                foreach (var room in roomList)
                {
                    if (room.Category == null && room.CategoryID > 0)
                    {
                        room.Category = _categoryRepository.GetCategoryById(room.CategoryID);
                    }
                    Rooms.Add(room);
                }

                OnPropertyChanged(nameof(TotalRoomsCount));
                OnPropertyChanged(nameof(AvailableRoomsCount));

                OnPropertyChanged(nameof(AvailableRoomsCount));

                if (roomList.Any() && SelectedRoom == null)
                {
                    SelectedRoom = Rooms.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки номеров: {ex.Message}");
            }
        }

        private void AddRoom(object parameter)
        {
            try
            {
                // Создаем новый номер
                var newRoom = new Room();

                // Получаем список категорий для выбора
                var categories = _categoryRepository.GetAllCategories().ToList();

                // Открываем окно добавления/редактирования номера
                var addWindow = new AddEditRoomWindow(newRoom, categories, true)
                {
                    Owner = Application.Current.MainWindow
                };

                if (addWindow.ShowDialog() == true)
                {
                    // Сохраняем номер
                    _roomRepository.AddRoom(newRoom);

                    LoadRooms(null);
                    ShowSuccessMessage("Номер успешно добавлен!");

                    // Выбор нового номера
                    SelectedRoom = Rooms.FirstOrDefault(r => r.RoomID == newRoom.RoomID);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при добавлении номера: {ex.Message}");
            }
        }

        public void EditRoom(object parameter)
        {
            try
            {
                if (SelectedRoom == null)
                {
                    ShowInfoMessage("Выберите номер для редактирования");
                    return;
                }

                var roomToEdit = new Room
                {
                    RoomID = SelectedRoom.RoomID,
                    RoomNumber = SelectedRoom.RoomNumber,
                    CategoryID = SelectedRoom.CategoryID,
                    Floor = SelectedRoom.Floor,
                    HasBalcony = SelectedRoom.HasBalcony,
                    IsAvailable = SelectedRoom.IsAvailable,
                    Category = SelectedRoom.Category
                };

                var categories = _categoryRepository.GetAllCategories().ToList();

                // Открытие окна редактирования
                var editWindow = new AddEditRoomWindow(roomToEdit, categories, false)
                {
                    Owner = Application.Current.MainWindow
                };

                if (editWindow.ShowDialog() == true)
                {
                    _roomRepository.UpdateRoom(roomToEdit);
                    LoadRooms(null);
                    SelectedRoom = Rooms.FirstOrDefault(r => r.RoomID == roomToEdit.RoomID);

                    ShowSuccessMessage("Данные номера успешно обновлены!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при редактировании номера: {ex.Message}");
            }
        }

        private void DeleteRoom(object parameter)
        {
            try
            {
                if (SelectedRoom == null)
                {
                    ShowInfoMessage("Выберите номер для удаления");
                    return;
                }

                string message =
                    $"Вы уверены, что хотите удалить номер:\n" +
                    $"Номер: {SelectedRoom.RoomNumber}\n" +
                    $"Этаж: {SelectedRoom.Floor}\n" +
                    $"Категория: {SelectedRoom.Category?.CategoryName}";

                var result = MessageBox.Show(
                    message,
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _roomRepository.DeleteRoom(SelectedRoom.RoomID);
                    LoadRooms(null);

                    ShowSuccessMessage("Номер успешно удален!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при удалении номера: {ex.Message}");
            }
        }

        private void RefreshData(object parameter)
        {
            LoadRooms(null);
        }

        private void PerformSearch(object parameter = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    LoadRooms(null);
                    return;
                }

                var searchResults = _roomRepository.SearchRooms(SearchText);
                Rooms.Clear();
                foreach (var room in searchResults)
                {
                    Rooms.Add(room);
                }

                if (searchResults.Any())
                {
                    SelectedRoom = Rooms.FirstOrDefault();
                }
                else
                {
                    SelectedRoom = null;
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
            LoadRooms(null);
        }

        private bool CanEditRoom(object parameter)
        {
            return SelectedRoom != null;
        }

        private bool CanDeleteRoom(object parameter)
        {
            return SelectedRoom != null;
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