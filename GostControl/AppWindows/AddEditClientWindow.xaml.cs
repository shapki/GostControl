using GostControl.AppModels;
using System.Windows;
using System.Windows.Controls;

namespace GostControl.AppWindows
{
    public partial class AddEditClientWindow : Window
    {
        private readonly Client _client;
        private readonly bool _isNew;

        public AddEditClientWindow(Client client, bool isNew)
        {
            InitializeComponent();
            _client = client;
            _isNew = isNew;

            Title = _isNew ? "Добавление клиента" : "Редактирование клиента";
            LoadClientData();
        }

        private void LoadClientData()
        {
            Title = _isNew ? "Добавление клиента" : "Редактирование клиента";

            var titleText = this.FindName("TitleText") as TextBlock;
            if (titleText != null)
            {
                titleText.Text = _isNew ? "Добавление клиента" : "Редактирование клиента";
            }

            LastNameTextBox.Text = _client.LastName;
            FirstNameTextBox.Text = _client.FirstName;
            MiddleNameTextBox.Text = _client.MiddleName;
            PassportSeriesTextBox.Text = _client.PassportSeries;
            PassportNumberTextBox.Text = _client.PassportNumber;
            PhoneTextBox.Text = _client.PhoneNumber;
            EmailTextBox.Text = _client.Email;

            if (_client.DateOfBirth.HasValue)
            {
                BirthDatePicker.SelectedDate = _client.DateOfBirth.Value;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля: Фамилия и Имя",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _client.LastName = LastNameTextBox.Text;
            _client.FirstName = FirstNameTextBox.Text;
            _client.MiddleName = MiddleNameTextBox.Text;
            _client.PassportSeries = PassportSeriesTextBox.Text;
            _client.PassportNumber = PassportNumberTextBox.Text;
            _client.PhoneNumber = PhoneTextBox.Text;
            _client.Email = EmailTextBox.Text;
            _client.DateOfBirth = BirthDatePicker.SelectedDate;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}