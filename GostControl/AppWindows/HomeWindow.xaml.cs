using GostControl.AppModels;
using GostControl.AppServices;
using System;
using System.Windows;

namespace GostControl.AppWindows
{
    public partial class HomeWindow : Window
    {
        private AppSettings _appSettings;

        public HomeWindow()
        {
            InitializeComponent();
            _appSettings = AppSettings.Load();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (settingsWindow.ShowDialog() == true)
            {
                _appSettings = AppSettings.Load();
            }
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            var clientsWindow = new ClientsWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            this.Hide();
            if (clientsWindow.ShowDialog() == true)
            {
                this.Show();
            }
            else
            {
                this.Show();
            }
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            var roomsWindow = new RoomsWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            this.Hide();
            if (roomsWindow.ShowDialog() == true)
            {
                this.Show();
            }
            else
            {
                this.Show();
            }
        }

        private void BookingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел 'Бронирования' находится в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CheckInsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел 'Заселения' находится в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CheckOutsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел 'Выселения' находится в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел 'Услуги' находится в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел 'Платежи' находится в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел 'Отчеты' находится в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}