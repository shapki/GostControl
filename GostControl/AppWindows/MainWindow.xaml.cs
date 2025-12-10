using GostControl.AppViewModels;
using System.Windows;

namespace GostControl.AppWindows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}