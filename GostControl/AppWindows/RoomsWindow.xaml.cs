using GostControl.AppModels;
using GostControl.AppServices;
using GostControl.AppViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace GostControl.AppWindows
{
    public partial class RoomsWindow : Window
    {
        private AppSettings _appSettings;

        public RoomsWindow()
        {
            InitializeComponent();
            _appSettings = AppSettings.Load();
            this.DataContext = new RoomsViewModel();
            ApplySettings();
        }

        private void ApplySettings()
        {
            try
            {
                if (!_appSettings.DoubleClickEditEnabled)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка применения настроек: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!_appSettings.DoubleClickEditEnabled)
                return;

            if (DataContext is RoomsViewModel vm && vm.SelectedRoom != null)
            {
                vm.EditRoom(null);
            }
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Статистика номеров находится в разработке...", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
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
                ApplySettings();
            }
        }

        private void ExportToCsvButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoomsViewModel vm)
            {
                if (vm.Rooms == null || vm.Rooms.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Экспортировать список номеров в CSV файл?\n\n" +
                                           "Файл будет сохранен в папке Загрузки.",
                                           "Экспорт в CSV",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Экспорт номеров в CSV находится в разработке", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoomsViewModel vm)
            {
                if (vm.Rooms == null || vm.Rooms.Count == 0)
                {
                    MessageBox.Show("Нет данных для печати", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Распечатать список номеров?", "Печать",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Печать номеров находится в разработке", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}