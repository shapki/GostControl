using GostControl.AppModels;
using GostControl.AppRepositories;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace GostControl.AppWindows
{
    public partial class EditClientWindow : Window
    {
        private readonly Client _client;
        private readonly ClientRepository _clientRepository;
        private readonly BookingRepository _bookingRepository;

        public EditClientWindow(Client client)
        {
            InitializeComponent();

            _client = client;
            _clientRepository = new ClientRepository();
            _bookingRepository = new BookingRepository();

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;

            LoadClientData();
            SetupValidation();

            if (client.ClientID == 0)
            {
                RegistrationDateText.Text = "Будет установлена автоматически";
                BookingsCountText.Text = "0";
                Title = "Добавление нового клиента";
            }
        }

        private void LoadClientData()
        {
            try
            {
                ClientIdText.Text = _client.ClientID.ToString();

                LastNameTextBox.Text = _client.LastName;
                FirstNameTextBox.Text = _client.FirstName;
                MiddleNameTextBox.Text = _client.MiddleName;
                PhoneTextBox.Text = _client.PhoneNumber;
                EmailTextBox.Text = _client.Email;
                PassportSeriesTextBox.Text = _client.PassportSeries;
                PassportNumberTextBox.Text = _client.PassportNumber;

                if (_client.DateOfBirth.HasValue)
                {
                    BirthDatePicker.SelectedDate = _client.DateOfBirth.Value;
                }

                RegistrationDateText.Text = _client.RegistrationDate.ToString("dd.MM.yyyy HH:mm");

                var bookingsCount = _bookingRepository.GetBookingsByClient(_client.ClientID).Count;
                BookingsCountText.Text = bookingsCount.ToString();

                LastNameTextBox.Focus();
                LastNameTextBox.SelectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupValidation()
        {
            try
            {
                LastNameTextBox.TextChanged += ValidateRequiredFields;
                FirstNameTextBox.TextChanged += ValidateRequiredFields;
                PhoneTextBox.TextChanged += ValidatePhone;
                EmailTextBox.TextChanged += ValidateEmail;

                ValidateAllFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка настройки валидации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValidateRequiredFields(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                bool lastNameValid = !string.IsNullOrWhiteSpace(LastNameTextBox.Text);
                bool firstNameValid = !string.IsNullOrWhiteSpace(FirstNameTextBox.Text);

                LastNameError.Visibility = lastNameValid ? Visibility.Collapsed : Visibility.Visible;
                FirstNameError.Visibility = firstNameValid ? Visibility.Collapsed : Visibility.Visible;

                UpdateSaveButton();
            }
            catch { }
        }

        private void ValidatePhone(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                string phone = PhoneTextBox.Text;
                bool isValid = IsValidPhone(phone);

                PhoneError.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
                UpdateSaveButton();
            }
            catch { }
        }

        private void ValidateEmail(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                string email = EmailTextBox.Text;

                if (string.IsNullOrWhiteSpace(email))
                {
                    EmailError.Visibility = Visibility.Collapsed;
                }
                else
                {
                    bool isValid = IsValidEmail(email);
                    EmailError.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
                }

                UpdateSaveButton();
            }
            catch { }
        }

        private bool IsValidPhone(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return false;

                return phone.Length >= 5;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return true;

                return email.Contains("@") && email.Contains(".");
            }
            catch
            {
                return false;
            }
        }

        private void ValidateAllFields()
        {
            try
            {
                ValidateRequiredFields(null, null);
                ValidatePhone(null, null);
                ValidateEmail(null, null);
            }
            catch { }
        }

        private void UpdateSaveButton()
        {
            try
            {
                bool isValid =
                    !string.IsNullOrWhiteSpace(LastNameTextBox.Text) &&
                    !string.IsNullOrWhiteSpace(FirstNameTextBox.Text) &&
                    IsValidPhone(PhoneTextBox.Text) &&
                    (string.IsNullOrWhiteSpace(EmailTextBox.Text) || IsValidEmail(EmailTextBox.Text));

                SaveButton.IsEnabled = isValid;
            }
            catch
            {
                SaveButton.IsEnabled = false;
            }
        }

        private bool HasChanges()
        {
            try
            {
                return _client.LastName != LastNameTextBox.Text ||
                       _client.FirstName != FirstNameTextBox.Text ||
                       _client.MiddleName != MiddleNameTextBox.Text ||
                       _client.PassportSeries != PassportSeriesTextBox.Text ||
                       _client.PassportNumber != PassportNumberTextBox.Text ||
                       _client.PhoneNumber != PhoneTextBox.Text ||
                       _client.Email != EmailTextBox.Text ||
                       _client.DateOfBirth != BirthDatePicker.SelectedDate;
            }
            catch
            {
                return false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!SaveButton.IsEnabled)
                    return;

                _client.LastName = LastNameTextBox.Text;
                _client.FirstName = FirstNameTextBox.Text;
                _client.MiddleName = MiddleNameTextBox.Text;
                _client.PhoneNumber = PhoneTextBox.Text;
                _client.Email = EmailTextBox.Text;
                _client.PassportSeries = PassportSeriesTextBox.Text;
                _client.PassportNumber = PassportNumberTextBox.Text;
                _client.DateOfBirth = BirthDatePicker.SelectedDate;

                if (_client.ClientID == 0)
                {
                    _client.RegistrationDate = DateTime.Now;

                    _clientRepository.AddClient(_client);

                    MessageBox.Show("Клиент успешно добавлен!",
                                  "Успех",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
                else
                {
                    _clientRepository.UpdateClient(_client);

                    MessageBox.Show("Данные клиента успешно обновлены!",
                                  "Успех",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (HasChanges())
                {
                    var result = MessageBox.Show("У вас есть несохраненные изменения. Вы уверены, что хотите выйти?",
                                               "Подтверждение",
                                               MessageBoxButton.YesNo,
                                               MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                        return;
                }

                DialogResult = false;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
            {
                CancelButton_Click(null, null);
            }
            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (SaveButton.IsEnabled)
                {
                    SaveButton_Click(null, null);
                }
                e.Handled = true;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (HasChanges() && DialogResult != true)
                {
                    var result = MessageBox.Show("У вас есть несохраненные изменения. Вы уверены, что хотите выйти?",
                                               "Подтверждение",
                                               MessageBoxButton.YesNo,
                                               MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch { }

            base.OnClosing(e);
        }
    }
}