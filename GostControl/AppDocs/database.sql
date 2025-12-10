USE master;
GO

CREATE DATABASE Shapkin_PKGH_HotelDB COLLATE Cyrillic_General_CI_AS;
GO

USE Shapkin_PKGH_HotelDB;
GO

CREATE TABLE RoomCategories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255),
    BasePrice DECIMAL(10,2) NOT NULL CHECK (BasePrice > 0),
    MaxCapacity INT NOT NULL CHECK (MaxCapacity > 0)
);
GO

CREATE TABLE Rooms (
    RoomID INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(10) NOT NULL UNIQUE,
    CategoryID INT NOT NULL,
    Floor INT NOT NULL CHECK (Floor >= 0),
    HasBalcony BIT DEFAULT 0,
    IsAvailable BIT DEFAULT 1,
    FOREIGN KEY (CategoryID) REFERENCES RoomCategories(CategoryID)
);
GO

CREATE TABLE AdditionalServices (
    ServiceID INT IDENTITY(1,1) PRIMARY KEY,
    ServiceName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    IsActive BIT DEFAULT 1
);
GO

CREATE TABLE Clients (
    ClientID INT IDENTITY(1,1) PRIMARY KEY,
    LastName NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50),
    PassportSeries NVARCHAR(4),
    PassportNumber NVARCHAR(6),
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(100),
    DateOfBirth DATE,
    RegistrationDate DATE DEFAULT GETDATE()
);
GO

CREATE TABLE Bookings (
    BookingID INT IDENTITY(1,1) PRIMARY KEY,
    ClientID INT NOT NULL,
    RoomID INT NOT NULL,
    BookingDate DATETIME DEFAULT GETDATE(),
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    TotalCost DECIMAL(10,2),
    Status NVARCHAR(20) DEFAULT N'Подтверждено' 
        CHECK (Status IN (N'Подтверждено', N'Отменено', N'Завершено')),
    Notes NVARCHAR(500),
    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID),
    CONSTRAINT CHK_Dates CHECK (CheckOutDate > CheckInDate)
);
GO

CREATE TABLE BookingServices (
    BookingServiceID INT IDENTITY(1,1) PRIMARY KEY,
    BookingID INT NOT NULL,
    ServiceID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1 CHECK (Quantity > 0),
    ServiceDate DATE,
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE,
    FOREIGN KEY (ServiceID) REFERENCES AdditionalServices(ServiceID)
);
GO

CREATE TABLE Employees (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    LastName NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50),
    Position NVARCHAR(100) NOT NULL,
    HireDate DATE NOT NULL,
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(100),
    IsActive BIT DEFAULT 1
);
GO

CREATE TABLE RoomCleaning (
    CleaningID INT IDENTITY(1,1) PRIMARY KEY,
    RoomID INT NOT NULL,
    EmployeeID INT NOT NULL,
    CleaningDate DATE NOT NULL DEFAULT GETDATE(),
    CleaningTime TIME,
    Status NVARCHAR(20) DEFAULT N'Запланировано'
        CHECK (Status IN (N'Запланировано', N'Выполнено', N'Отменено')),
    Notes NVARCHAR(255),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);
GO

CREATE INDEX IX_Rooms_Category ON Rooms(CategoryID);
CREATE INDEX IX_Bookings_Client ON Bookings(ClientID);
CREATE INDEX IX_Bookings_Room ON Bookings(RoomID);
CREATE INDEX IX_Bookings_Dates ON Bookings(CheckInDate, CheckOutDate);
CREATE INDEX IX_Clients_Name ON Clients(LastName, FirstName);
CREATE INDEX IX_BookingServices_Booking ON BookingServices(BookingID);
GO


INSERT INTO RoomCategories (CategoryName, Description, BasePrice, MaxCapacity) 
VALUES 
    (N'Эконом', N'Номер с минимальными удобствами', 1500.00, 2),
    (N'Стандарт', N'Стандартный номер с TV и кондиционером', 2500.00, 2),
    (N'Полулюкс', N'Просторный номер с улучшенной отделкой', 4000.00, 3),
    (N'Люкс', N'Номер повышенной комфортности с гостиной', 6000.00, 4),
    (N'Президентский', N'Самый роскошный номер отеля', 12000.00, 4);
GO

INSERT INTO Rooms (RoomNumber, CategoryID, Floor, HasBalcony, IsAvailable) 
VALUES 
    ('101', 1, 1, 0, 1),
    ('102', 1, 1, 0, 1),
    ('201', 2, 2, 1, 1),
    ('202', 2, 2, 1, 1),
    ('301', 3, 3, 1, 1),
    ('302', 3, 3, 1, 1),
    ('401', 4, 4, 1, 1),
    ('501', 5, 5, 1, 1);
GO

INSERT INTO AdditionalServices (ServiceName, Description, Price) 
VALUES 
    (N'Завтрак', N'Континентальный завтрак', 300.00),
    (N'Ужин', N'Трехразовое питание', 800.00),
    (N'Трансфер', N'Трансфер из/в аэропорт', 1500.00),
    (N'SPA', N'Посещение SPA-центра', 2000.00),
    (N'Прачечная', N'Стирка и глажка одежды', 500.00),
    (N'Парковка', N'Место на охраняемой парковке', 300.00);
GO

INSERT INTO Clients (LastName, FirstName, MiddleName, PassportSeries, PassportNumber, PhoneNumber, Email, DateOfBirth) 
VALUES 
    (N'Иванов', N'Иван', N'Иванович', '1234', '567890', '+79161234567', 'ivanov@mail.ru', '1985-05-15'),
    (N'Петрова', N'Мария', N'Сергеевна', '2345', '678901', '+79162345678', 'petrova@gmail.com', '1990-08-22'),
    (N'Сидоров', N'Алексей', N'Петрович', '3456', '789012', '+79173456789', 'sidorov@yandex.ru', '1978-12-10'),
    (N'Козлова', N'Елена', N'Владимировна', '4567', '890123', '+79184567890', 'kozlova@mail.ru', '1995-03-30'),
    (N'Николаев', N'Дмитрий', N'Александрович', '5678', '901234', '+79195678901', 'nikolaev@gmail.com', '1982-11-05');
GO

INSERT INTO Employees (LastName, FirstName, MiddleName, Position, HireDate, PhoneNumber, Email) 
VALUES 
    (N'Смирнов', N'Андрей', N'Викторович', N'Администратор', '2020-01-15', '+79211234567', 'smirnov@hotel.ru'),
    (N'Васильева', N'Ольга', N'Игоревна', N'Горничная', '2021-03-20', '+79212345678', 'vasileva@hotel.ru'),
    (N'Кузнецов', N'Сергей', N'Михайлович', N'Портье', '2019-11-10', '+79213456789', 'kuznetsov@hotel.ru'),
    (N'Федорова', N'Анна', N'Дмитриевна', N'Менеджер', '2018-06-05', '+79214567890', 'fedorova@hotel.ru');
GO

INSERT INTO Bookings (ClientID, RoomID, BookingDate, CheckInDate, CheckOutDate, TotalCost, Status) 
VALUES 
    (1, 3, DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, 10, GETDATE()), DATEADD(DAY, 14, GETDATE()), 10000.00, N'Подтверждено'),
    (2, 5, DATEADD(DAY, -28, GETDATE()), DATEADD(DAY, 20, GETDATE()), DATEADD(DAY, 25, GETDATE()), 20000.00, N'Подтверждено'),
    (3, 1, DATEADD(DAY, -25, GETDATE()), DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, -3, GETDATE()), 3000.00, N'Завершено'),
    (4, 7, DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, 40, GETDATE()), DATEADD(DAY, 45, GETDATE()), 30000.00, N'Подтверждено');
GO

INSERT INTO BookingServices (BookingID, ServiceID, Quantity, ServiceDate) 
VALUES 
    (1, 1, 4, DATEADD(DAY, 11, GETDATE())),
    (1, 6, 4, NULL),
    (2, 1, 5, NULL),
    (2, 4, 2, DATEADD(DAY, 21, GETDATE())),
    (4, 3, 1, DATEADD(DAY, 41, GETDATE()));
GO

INSERT INTO RoomCleaning (RoomID, EmployeeID, CleaningDate, Status) 
VALUES 
    (1, 2, DATEADD(DAY, -5, GETDATE()), N'Выполнено'),
    (3, 2, DATEADD(DAY, -4, GETDATE()), N'Выполнено'),
    (5, 2, DATEADD(DAY, 2, GETDATE()), N'Запланировано'),
    (7, 2, DATEADD(DAY, 3, GETDATE()), N'Запланировано');
GO

CREATE VIEW ActiveBookingsView AS
SELECT 
    b.BookingID,
    c.LastName + ' ' + c.FirstName + ' ' + ISNULL(c.MiddleName, '') AS ClientFullName,
    r.RoomNumber,
    rc.CategoryName,
    b.CheckInDate,
    b.CheckOutDate,
    b.TotalCost,
    b.Status
FROM Bookings b
JOIN Clients c ON b.ClientID = c.ClientID
JOIN Rooms r ON b.RoomID = r.RoomID
JOIN RoomCategories rc ON r.CategoryID = rc.CategoryID
WHERE b.Status = N'Подтверждено';
GO

CREATE VIEW AvailableRoomsView AS
SELECT 
    r.RoomID,
    r.RoomNumber,
    rc.CategoryName,
    rc.BasePrice,
    r.Floor,
    CASE WHEN r.HasBalcony = 1 THEN N'Да' ELSE N'Нет' END AS HasBalcony
FROM Rooms r
JOIN RoomCategories rc ON r.CategoryID = rc.CategoryID
WHERE r.IsAvailable = 1;
GO

CREATE PROCEDURE AddNewClient
    @LastName NVARCHAR(50),
    @FirstName NVARCHAR(50),
    @MiddleName NVARCHAR(50) = NULL,
    @PassportSeries NVARCHAR(4) = NULL,
    @PassportNumber NVARCHAR(6) = NULL,
    @PhoneNumber NVARCHAR(20) = NULL,
    @Email NVARCHAR(100) = NULL,
    @DateOfBirth DATE = NULL
AS
BEGIN
    INSERT INTO Clients (LastName, FirstName, MiddleName, PassportSeries, PassportNumber, 
                         PhoneNumber, Email, DateOfBirth)
    VALUES (@LastName, @FirstName, @MiddleName, @PassportSeries, @PassportNumber,
            @PhoneNumber, @Email, @DateOfBirth);
    
    RETURN SCOPE_IDENTITY();
END;
GO

CREATE TRIGGER UpdateRoomAvailability
ON Bookings
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @RoomID INT, @Status NVARCHAR(20);
    
    SELECT @RoomID = RoomID, @Status = Status 
    FROM inserted;
    
    IF @Status IN (N'Подтверждено', N'Завершено')
    BEGIN
        UPDATE Rooms 
        SET IsAvailable = 0 
        WHERE RoomID = @RoomID;
    END
    ELSE IF @Status = N'Отменено'
    BEGIN
        UPDATE Rooms 
        SET IsAvailable = 1 
        WHERE RoomID = @RoomID;
    END
END;
GO