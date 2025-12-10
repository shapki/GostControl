using GostControl.AppModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GostControl.AppServices
{
    public class LocalDataService
    {
        private static LocalDataService _instance;
        public static LocalDataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalDataService();
                }
                return _instance;
            }
        }

        // Коллекции данных в памяти
        public ObservableCollection<Client> Clients { get; private set; }
        public ObservableCollection<Room> Rooms { get; private set; }
        public ObservableCollection<RoomCategory> RoomCategories { get; private set; }
        public ObservableCollection<Booking> Bookings { get; private set; }
        public ObservableCollection<AdditionalService> AdditionalServices { get; private set; }
        public ObservableCollection<Employee> Employees { get; private set; }
        public ObservableCollection<RoomCleaning> RoomCleanings { get; private set; }

        private LocalDataService()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            // Инициализируем коллекции
            Clients = new ObservableCollection<Client>();
            Rooms = new ObservableCollection<Room>();
            RoomCategories = new ObservableCollection<RoomCategory>();
            Bookings = new ObservableCollection<Booking>();
            AdditionalServices = new ObservableCollection<AdditionalService>();
            Employees = new ObservableCollection<Employee>();
            RoomCleanings = new ObservableCollection<RoomCleaning>();

            // Заполняем тестовыми данными
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Категории номеров
            var categories = new List<RoomCategory>
            {
                new RoomCategory { CategoryID = 1, CategoryName = "Эконом", Description = "Номер с минимальными удобствами", BasePrice = 1500.00m, MaxCapacity = 2 },
                new RoomCategory { CategoryID = 2, CategoryName = "Стандарт", Description = "Стандартный номер с TV и кондиционером", BasePrice = 2500.00m, MaxCapacity = 2 },
                new RoomCategory { CategoryID = 3, CategoryName = "Полулюкс", Description = "Просторный номер с улучшенной отделкой", BasePrice = 4000.00m, MaxCapacity = 3 },
                new RoomCategory { CategoryID = 4, CategoryName = "Люкс", Description = "Номер повышенной комфортности с гостиной", BasePrice = 6000.00m, MaxCapacity = 4 },
                new RoomCategory { CategoryID = 5, CategoryName = "Президентский", Description = "Самый роскошный номер отеля", BasePrice = 12000.00m, MaxCapacity = 4 }
            };

            foreach (var category in categories)
            {
                RoomCategories.Add(category);
            }

            // Номера
            var rooms = new List<Room>
            {
                new Room { RoomID = 1, RoomNumber = "101", CategoryID = 1, Floor = 1, HasBalcony = false, IsAvailable = true },
                new Room { RoomID = 2, RoomNumber = "102", CategoryID = 1, Floor = 1, HasBalcony = false, IsAvailable = true },
                new Room { RoomID = 3, RoomNumber = "201", CategoryID = 2, Floor = 2, HasBalcony = true, IsAvailable = false },
                new Room { RoomID = 4, RoomNumber = "202", CategoryID = 2, Floor = 2, HasBalcony = true, IsAvailable = true },
                new Room { RoomID = 5, RoomNumber = "301", CategoryID = 3, Floor = 3, HasBalcony = true, IsAvailable = false },
                new Room { RoomID = 6, RoomNumber = "302", CategoryID = 3, Floor = 3, HasBalcony = true, IsAvailable = true },
                new Room { RoomID = 7, RoomNumber = "401", CategoryID = 4, Floor = 4, HasBalcony = true, IsAvailable = false },
                new Room { RoomID = 8, RoomNumber = "501", CategoryID = 5, Floor = 5, HasBalcony = true, IsAvailable = true }
            };

            // Привязываем категории к номерам
            foreach (var room in rooms)
            {
                room.Category = categories.FirstOrDefault(c => c.CategoryID == room.CategoryID);
                Rooms.Add(room);
            }

            // Дополнительные услуги
            var services = new List<AdditionalService>
            {
                new AdditionalService { ServiceID = 1, ServiceName = "Завтрак", Description = "Континентальный завтрак", Price = 300.00m, IsActive = true },
                new AdditionalService { ServiceID = 2, ServiceName = "Ужин", Description = "Трехразовое питание", Price = 800.00m, IsActive = true },
                new AdditionalService { ServiceID = 3, ServiceName = "Трансфер", Description = "Трансфер из/в аэропорт", Price = 1500.00m, IsActive = true },
                new AdditionalService { ServiceID = 4, ServiceName = "SPA", Description = "Посещение SPA-центра", Price = 2000.00m, IsActive = true },
                new AdditionalService { ServiceID = 5, ServiceName = "Прачечная", Description = "Стирка и глажка одежды", Price = 500.00m, IsActive = true },
                new AdditionalService { ServiceID = 6, ServiceName = "Парковка", Description = "Место на охраняемой парковке", Price = 300.00m, IsActive = true }
            };

            foreach (var service in services)
            {
                AdditionalServices.Add(service);
            }

            // Клиенты
            var clients = new List<Client>
            {
                new Client { ClientID = 1, LastName = "Иванов", FirstName = "Иван", MiddleName = "Иванович",
                            PassportSeries = "1234", PassportNumber = "567890",
                            PhoneNumber = "+79161234567", Email = "ivanov@mail.ru",
                            DateOfBirth = new DateTime(1985, 5, 15), RegistrationDate = DateTime.Now.AddDays(-30) },
                new Client { ClientID = 2, LastName = "Петрова", FirstName = "Мария", MiddleName = "Сергеевна",
                            PassportSeries = "2345", PassportNumber = "678901",
                            PhoneNumber = "+79162345678", Email = "petrova@gmail.com",
                            DateOfBirth = new DateTime(1990, 8, 22), RegistrationDate = DateTime.Now.AddDays(-25) },
                new Client { ClientID = 3, LastName = "Сидоров", FirstName = "Алексей", MiddleName = "Петрович",
                            PassportSeries = "3456", PassportNumber = "789012",
                            PhoneNumber = "+79173456789", Email = "sidorov@yandex.ru",
                            DateOfBirth = new DateTime(1978, 12, 10), RegistrationDate = DateTime.Now.AddDays(-20) },
                new Client { ClientID = 4, LastName = "Козлова", FirstName = "Елена", MiddleName = "Владимировна",
                            PassportSeries = "4567", PassportNumber = "890123",
                            PhoneNumber = "+79184567890", Email = "kozlova@mail.ru",
                            DateOfBirth = new DateTime(1995, 3, 30), RegistrationDate = DateTime.Now.AddDays(-15) },
                new Client { ClientID = 5, LastName = "Николаев", FirstName = "Дмитрий", MiddleName = "Александрович",
                            PassportSeries = "5678", PassportNumber = "901234",
                            PhoneNumber = "+79195678901", Email = "nikolaev@gmail.com",
                            DateOfBirth = new DateTime(1982, 11, 5), RegistrationDate = DateTime.Now.AddDays(-10) }
            };

            foreach (var client in clients)
            {
                Clients.Add(client);
            }

            // Сотрудники
            var employees = new List<Employee>
            {
                new Employee { EmployeeID = 1, LastName = "Смирнов", FirstName = "Андрей", MiddleName = "Викторович",
                              Position = "Администратор", HireDate = new DateTime(2020, 1, 15),
                              PhoneNumber = "+79211234567", Email = "smirnov@hotel.ru", IsActive = true },
                new Employee { EmployeeID = 2, LastName = "Васильева", FirstName = "Ольга", MiddleName = "Игоревна",
                              Position = "Горничная", HireDate = new DateTime(2021, 3, 20),
                              PhoneNumber = "+79212345678", Email = "vasileva@hotel.ru", IsActive = true },
                new Employee { EmployeeID = 3, LastName = "Кузнецов", FirstName = "Сергей", MiddleName = "Михайлович",
                              Position = "Портье", HireDate = new DateTime(2019, 11, 10),
                              PhoneNumber = "+79213456789", Email = "kuznetsov@hotel.ru", IsActive = true },
                new Employee { EmployeeID = 4, LastName = "Федорова", FirstName = "Анна", MiddleName = "Дмитриевна",
                              Position = "Менеджер", HireDate = new DateTime(2018, 6, 5),
                              PhoneNumber = "+79214567890", Email = "fedorova@hotel.ru", IsActive = true }
            };

            foreach (var employee in employees)
            {
                Employees.Add(employee);
            }

            // Бронирования
            var bookings = new List<Booking>
            {
                new Booking { BookingID = 1, ClientID = 1, RoomID = 3,
                             BookingDate = DateTime.Now.AddDays(-30),
                             CheckInDate = DateTime.Now.AddDays(10),
                             CheckOutDate = DateTime.Now.AddDays(14),
                             TotalCost = 10000.00m, Status = "Подтверждено" },
                new Booking { BookingID = 2, ClientID = 2, RoomID = 5,
                             BookingDate = DateTime.Now.AddDays(-28),
                             CheckInDate = DateTime.Now.AddDays(20),
                             CheckOutDate = DateTime.Now.AddDays(25),
                             TotalCost = 20000.00m, Status = "Подтверждено" },
                new Booking { BookingID = 3, ClientID = 3, RoomID = 1,
                             BookingDate = DateTime.Now.AddDays(-25),
                             CheckInDate = DateTime.Now.AddDays(-5),
                             CheckOutDate = DateTime.Now.AddDays(-3),
                             TotalCost = 3000.00m, Status = "Завершено" },
                new Booking { BookingID = 4, ClientID = 4, RoomID = 7,
                             BookingDate = DateTime.Now.AddDays(-20),
                             CheckInDate = DateTime.Now.AddDays(40),
                             CheckOutDate = DateTime.Now.AddDays(45),
                             TotalCost = 30000.00m, Status = "Подтверждено" }
            };

            // Привязываем клиентов и номера к бронированиям
            foreach (var booking in bookings)
            {
                booking.Client = clients.FirstOrDefault(c => c.ClientID == booking.ClientID);
                booking.Room = rooms.FirstOrDefault(r => r.RoomID == booking.RoomID);
                Bookings.Add(booking);
            }

            // Уборка номеров
            var cleanings = new List<RoomCleaning>
            {
                new RoomCleaning { CleaningID = 1, RoomID = 1, EmployeeID = 2,
                                 CleaningDate = DateTime.Now.AddDays(-5),
                                 Status = "Выполнено" },
                new RoomCleaning { CleaningID = 2, RoomID = 3, EmployeeID = 2,
                                 CleaningDate = DateTime.Now.AddDays(-4),
                                 Status = "Выполнено" },
                new RoomCleaning { CleaningID = 3, RoomID = 5, EmployeeID = 2,
                                 CleaningDate = DateTime.Now.AddDays(2),
                                 Status = "Запланировано" },
                new RoomCleaning { CleaningID = 4, RoomID = 7, EmployeeID = 2,
                                 CleaningDate = DateTime.Now.AddDays(3),
                                 Status = "Запланировано" }
            };

            // Привязываем комнаты и сотрудников к уборкам
            foreach (var cleaning in cleanings)
            {
                cleaning.Room = rooms.FirstOrDefault(r => r.RoomID == cleaning.RoomID);
                cleaning.Employee = employees.FirstOrDefault(e => e.EmployeeID == cleaning.EmployeeID);
                RoomCleanings.Add(cleaning);
            }
        }

        // Методы для работы с данными
        public int GetNextClientId()
        {
            return Clients.Any() ? Clients.Max(c => c.ClientID) + 1 : 1;
        }

        public int GetNextRoomId()
        {
            return Rooms.Any() ? Rooms.Max(r => r.RoomID) + 1 : 1;
        }

        public int GetNextBookingId()
        {
            return Bookings.Any() ? Bookings.Max(b => b.BookingID) + 1 : 1;
        }

        public int GetNextServiceId()
        {
            return AdditionalServices.Any() ? AdditionalServices.Max(s => s.ServiceID) + 1 : 1;
        }

        public int GetNextEmployeeId()
        {
            return Employees.Any() ? Employees.Max(e => e.EmployeeID) + 1 : 1;
        }

        public int GetNextCleaningId()
        {
            return RoomCleanings.Any() ? RoomCleanings.Max(c => c.CleaningID) + 1 : 1;
        }

        // Получение связанных данных
        public Client GetClientById(int id)
        {
            return Clients.FirstOrDefault(c => c.ClientID == id);
        }

        public Room GetRoomById(int id)
        {
            return Rooms.FirstOrDefault(r => r.RoomID == id);
        }

        public RoomCategory GetCategoryById(int id)
        {
            return RoomCategories.FirstOrDefault(c => c.CategoryID == id);
        }

        public AdditionalService GetServiceById(int id)
        {
            return AdditionalServices.FirstOrDefault(s => s.ServiceID == id);
        }

        public Employee GetEmployeeById(int id)
        {
            return Employees.FirstOrDefault(e => e.EmployeeID == id);
        }

        public Booking GetBookingById(int id)
        {
            return Bookings.FirstOrDefault(b => b.BookingID == id);
        }

        public RoomCleaning GetCleaningById(int id)
        {
            return RoomCleanings.FirstOrDefault(c => c.CleaningID == id);
        }

        // Поиск данных
        public List<Client> SearchClients(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return Clients.ToList();

            return Clients
                .Where(c => c.LastName.ToLower().Contains(searchText.ToLower()) ||
                           c.FirstName.ToLower().Contains(searchText.ToLower()) ||
                           (c.MiddleName != null && c.MiddleName.ToLower().Contains(searchText.ToLower())) ||
                           c.PhoneNumber.Contains(searchText) ||
                           c.Email.ToLower().Contains(searchText.ToLower()))
                .ToList();
        }

        // Получение бронирований клиента
        public List<Booking> GetBookingsByClient(int clientId)
        {
            return Bookings.Where(b => b.ClientID == clientId).ToList();
        }

        // Получение доступных номеров
        public List<Room> GetAvailableRooms()
        {
            return Rooms.Where(r => r.IsAvailable).ToList();
        }

        // Получение активных бронирований
        public List<Booking> GetActiveBookings()
        {
            return Bookings.Where(b => b.Status == "Подтверждено").ToList();
        }
    }
}