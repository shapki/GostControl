using GostControl.AppModels;
using GostControl.AppServices;
using GostControl.AppViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace GostControl.AppWindows
{
    public partial class MainWindow : Window
    {
        private AppSettings _appSettings;

        public MainWindow()
        {
            InitializeComponent();
            _appSettings = AppSettings.Load();
            this.DataContext = new MainViewModel();
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

            if (DataContext is MainViewModel vm && vm.SelectedClient != null)
            {
                vm.EditClient(null);
            }
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция статистики в разработке...", "Информация",
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

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.Clients == null || vm.Clients.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Спросить пользователя о сохранении
                var result = MessageBox.Show("Экспортировать список клиентов в CSV файл?\n\n" +
                                           "Файл будет сохранен в папке Загрузки.",
                                           "Экспорт в CSV",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    ExcelExportService.ExportClients(vm.Clients);
                }
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.Clients == null || vm.Clients.Count == 0)
                {
                    MessageBox.Show("Нет данных для печати", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Спросить пользователя о печати
                var result = MessageBox.Show("Распечатать список клиентов?", "Печать",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    PrintService.PrintClients(vm.Clients);
                }
            }
        }
    }
}