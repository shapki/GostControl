using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using GostControl.AppModels;

namespace GostControl.AppData
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext() : base()
        {
        }

        public HotelDbContext(DbContextOptions<HotelDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomCategory> RoomCategories { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<AdditionalService> AdditionalServices { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RoomCleaning> RoomCleanings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = GetConnectionString();

                SqlServerDbContextOptionsExtensions.UseSqlServer(
                    optionsBuilder,
                    connectionString,
                    sqlServerOptions => sqlServerOptions.MigrationsAssembly("GostControl")
                );
            }
        }

        private string GetConnectionString()
        {
            try
            {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings["HotelDBConnectionString"];

                if (connectionStringSettings == null)
                {
                    throw new ConfigurationErrorsException(
                        "Строка подключения не найдена..");
                }

                var connectionString = connectionStringSettings.ConnectionString;

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ConfigurationErrorsException(
                        "Строка подключения пуста.");
                }

                return connectionString;
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(
                    $"Ошибка при чтении строки подключения: {ex.Message}", ex);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.ClientID);
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50);
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);
                entity.Property(e => e.Email)
                    .HasMaxLength(100);
                entity.Property(e => e.PassportSeries)
                    .HasMaxLength(4);
                entity.Property(e => e.PassportNumber)
                    .HasMaxLength(6);
                entity.Property(e => e.RegistrationDate)
                    .HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.RoomID);
                entity.Property(e => e.RoomNumber)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.HasIndex(e => e.RoomNumber)
                    .IsUnique();
                entity.Property(e => e.IsAvailable)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingID);
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Подтверждено");
                entity.Property(e => e.Notes)
                    .HasMaxLength(500);
                entity.Property(e => e.TotalCost)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.BookingDate)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasCheckConstraint("CHK_Dates", "CheckOutDate > CheckInDate");
            });

            modelBuilder.Entity<RoomCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryID);
                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.BasePrice)
                    .HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<AdditionalService>(entity =>
            {
                entity.HasKey(e => e.ServiceID);
                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<BookingService>(entity =>
            {
                entity.HasKey(e => e.BookingServiceID);
                entity.Property(e => e.Quantity)
                    .HasDefaultValue(1);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeID);
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<RoomCleaning>(entity =>
            {
                entity.HasKey(e => e.CleaningID);
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Запланировано");
            });

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Category)
                .WithMany()
                .HasForeignKey(r => r.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Client)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.ClientID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany()
                .HasForeignKey(b => b.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingServices)
                .HasForeignKey(bs => bs.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Service)
                .WithMany()
                .HasForeignKey(bs => bs.ServiceID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomCleaning>()
                .HasOne(rc => rc.Room)
                .WithMany()
                .HasForeignKey(rc => rc.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomCleaning>()
                .HasOne(rc => rc.Employee)
                .WithMany()
                .HasForeignKey(rc => rc.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}