using System.Windows;

namespace GostControl.AppServices
{
    public static class MessageService
    {
        public static void ShowError(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public static void ShowInfo(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        public static void ShowWarning(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }
    }
}