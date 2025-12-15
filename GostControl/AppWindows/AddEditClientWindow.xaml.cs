using System;
using System.Windows;
using GostControl.AppModels;

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
            TitleText.Text = _isNew ? "Добавление клиента" : "Редактирование клиента";

            LoadClientData();
            SetupRegistrationInfo();
        }

        private void LoadClientData()
        {
            // Загрузка данных только если это редактирование существующего клиента
            if (!_isNew && _client != null)
            {
                LastNameTextBox.Text = _client.LastName ?? "";
                FirstNameTextBox.Text = _client.FirstName ?? "";
                MiddleNameTextBox.Text = _client.MiddleName ?? "";
                PhoneTextBox.Text = _client.PhoneNumber ?? "";
                EmailTextBox.Text = _client.Email ?? "";
                PassportSeriesTextBox.Text = _client.PassportSeries ?? "";
                PassportNumberTextBox.Text = _client.PassportNumber ?? "";

                if (_client.DateOfBirth.HasValue)
                {
                    BirthDatePicker.SelectedDate = _client.DateOfBirth.Value;
                }
            }
        }

        private void SetupRegistrationInfo()
        {
            // Показываем информацию о регистрации только для редактирования существующего клиента
            if (!_isNew && _client != null)
            {
                RegistrationInfoBorder.Visibility = Visibility.Visible;

                // Устанавливаем ID клиента
                ClientIdText.Text = _client.ClientID > 0 ? _client.ClientID.ToString() : "Нет данных";

                // Устанавливаем дату регистрации
                if (_client.RegistrationDate != default)
                {
                    RegistrationDateText.Text = _client.RegistrationDate.ToString("dd.MM.yyyy");
                }
                else
                {
                    RegistrationDateText.Text = "Нет данных";
                }
            }
            else
            {
                RegistrationInfoBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля: Фамилия, Имя, Телефон.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Сохранение данных в объект клиента
            _client.LastName = LastNameTextBox.Text.Trim();
            _client.FirstName = FirstNameTextBox.Text.Trim();
            _client.MiddleName = MiddleNameTextBox.Text?.Trim();
            _client.PhoneNumber = PhoneTextBox.Text.Trim();
            _client.Email = EmailTextBox.Text?.Trim();
            _client.PassportSeries = PassportSeriesTextBox.Text?.Trim();
            _client.PassportNumber = PassportNumberTextBox.Text?.Trim();
            _client.DateOfBirth = BirthDatePicker.SelectedDate;

            // Если это новый клиент - установить дату регистрации
            if (_isNew)
            {
                _client.RegistrationDate = DateTime.Now;
            }

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