using System;
using System.Windows;
using GostControl.AppModels;

namespace GostControl.AppWindows
{
    public partial class AddEditClientWindow : Window
    {
        public Client Client { get; private set; }

        public AddEditClientWindow()
        {
            InitializeComponent();
            Client = new Client();
            TitleText.Text = "Добавление клиента";
        }

        public AddEditClientWindow(Client clientToEdit) : this()
        {
            if (clientToEdit == null)
                throw new ArgumentNullException(nameof(clientToEdit));

            TitleText.Text = "Редактирование клиента";

            LastNameTextBox.Text = clientToEdit.LastName ?? "";
            FirstNameTextBox.Text = clientToEdit.FirstName ?? "";
            MiddleNameTextBox.Text = clientToEdit.MiddleName ?? "";
            BirthDatePicker.SelectedDate = clientToEdit.DateOfBirth;
            PhoneTextBox.Text = clientToEdit.PhoneNumber ?? "";
            EmailTextBox.Text = clientToEdit.Email ?? "";
            PassportSeriesTextBox.Text = clientToEdit.PassportSeries ?? "";
            PassportNumberTextBox.Text = clientToEdit.PassportNumber ?? "";

            Client.ClientID = clientToEdit.ClientID;
            Client.RegistrationDate = clientToEdit.RegistrationDate;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля: Фамилия, Имя, Телефон.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Client.LastName = LastNameTextBox.Text.Trim();
            Client.FirstName = FirstNameTextBox.Text.Trim();
            Client.MiddleName = MiddleNameTextBox.Text?.Trim();
            Client.DateOfBirth = BirthDatePicker.SelectedDate;
            Client.PhoneNumber = PhoneTextBox.Text.Trim();
            Client.Email = EmailTextBox.Text?.Trim();
            Client.PassportSeries = PassportSeriesTextBox.Text?.Trim();
            Client.PassportNumber = PassportNumberTextBox.Text?.Trim();

            if (Client.RegistrationDate == default)
                Client.RegistrationDate = DateTime.Now;

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