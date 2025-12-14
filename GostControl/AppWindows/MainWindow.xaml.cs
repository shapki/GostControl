using GostControl.AppModels;
using GostControl.AppViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GostControl.AppWindows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.SelectedClient != null)
            {
                var window = new AddEditClientWindow(vm.SelectedClient)
                {
                    Owner = this
                };

                if (window.ShowDialog() == true)
                {
                    // Обновление данных у существующего клиента (по ссылке)
                    var updated = window.Client;
                    vm.SelectedClient.LastName = updated.LastName;
                    vm.SelectedClient.FirstName = updated.FirstName;
                    vm.SelectedClient.MiddleName = updated.MiddleName;
                    vm.SelectedClient.DateOfBirth = updated.DateOfBirth;
                    vm.SelectedClient.PhoneNumber = updated.PhoneNumber;
                    vm.SelectedClient.Email = updated.Email;
                    vm.SelectedClient.PassportSeries = updated.PassportSeries;
                    vm.SelectedClient.PassportNumber = updated.PassportNumber;
                }
            }
        }
    }
}