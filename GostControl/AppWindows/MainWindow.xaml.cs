using GostControl.AppViewModels;
using System.Windows;
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
            var viewModel = DataContext as MainViewModel;
            viewModel?.EditClient(null);
        }
    }
}