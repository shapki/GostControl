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

        private string _originalLastName;
        private string _originalFirstName;
        private string _originalMiddleName;
        private string _originalPhone;
        private string _originalEmail;
        private string _originalPassportSeries;
        private string _originalPassportNumber;
        private DateTime? _originalDateOfBirth;

        public EditClientWindow(Client client)
        {
            InitializeComponent();

            _client = client;
            _clientRepository = new ClientRepository();
            _bookingRepository = new BookingRepository();

            SaveOriginalValues();

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;

            LoadClientData();
            SetupValidation();
        }

        private void SaveOriginalValues()
        {
            _originalLastName = _client.LastName;
            _originalFirstName = _client.FirstName;
            _originalMiddleName = _client.MiddleName;
            _originalPhone = _client.PhoneNumber;
            _originalEmail = _client.Email;
            _originalPassportSeries = _client.PassportSeries;
            _originalPassportNumber = _client.PassportNumber;
            _originalDateOfBirth = _client.DateOfBirth;
        }

        private void LoadClientData()
        {
            try
            {
                ClientIdText.Text = _client.ClientID > 0 ? _client.ClientID.ToString() : "Новый";

                if (_client.ClientID == 0)
                {
                    Title = "Добавление нового клиента";
                }
                else
                {
                    Title = $"Редактирование клиента #{_client.ClientID}";
                }

                LastNameTextBox.Text = _originalLastName ?? "";
                FirstNameTextBox.Text = _originalFirstName ?? "";
                MiddleNameTextBox.Text = _originalMiddleName ?? "";
                PhoneTextBox.Text = _originalPhone ?? "";
                EmailTextBox.Text = _originalEmail ?? "";
                PassportSeriesTextBox.Text = _originalPassportSeries ?? "";
                PassportNumberTextBox.Text = _originalPassportNumber ?? "";

                if (_originalDateOfBirth.HasValue)
                {
                    BirthDatePicker.SelectedDate = _originalDateOfBirth.Value;
                }
                else
                {
                    BirthDatePicker.SelectedDate = null;
                }

                if (_client.ClientID > 0)
                {
                    RegistrationDateText.Text = _client.RegistrationDate.ToString("dd.MM.yyyy");
                }
                else
                {
                    RegistrationDateText.Text = "Будет установлена автоматически";
                }

                if (_client.ClientID > 0)
                {
                    var bookingsCount = _bookingRepository.GetBookingsByClient(_client.ClientID).Count;
                    BookingsCountText.Text = bookingsCount.ToString();
                }
                else
                {
                    BookingsCountText.Text = "0";
                }

                LastNameTextBox.Focus();
                LastNameTextBox.SelectAll();

                UpdateSaveButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Ошибка настройки валидации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

                string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
                return digitsOnly.Length >= 5;
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

                return email.Contains("@") && email.Contains(".") && email.Length > 5;
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
                return _originalLastName != LastNameTextBox.Text ||
                       _originalFirstName != FirstNameTextBox.Text ||
                       _originalMiddleName != MiddleNameTextBox.Text ||
                       _originalPassportSeries != PassportSeriesTextBox.Text ||
                       _originalPassportNumber != PassportNumberTextBox.Text ||
                       _originalPhone != PhoneTextBox.Text ||
                       _originalEmail != EmailTextBox.Text ||
                       _originalDateOfBirth != BirthDatePicker.SelectedDate;
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

                _client.LastName = LastNameTextBox.Text.Trim();
                _client.FirstName = FirstNameTextBox.Text.Trim();
                _client.MiddleName = MiddleNameTextBox.Text.Trim();
                _client.PhoneNumber = PhoneTextBox.Text.Trim();
                _client.Email = EmailTextBox.Text.Trim();
                _client.PassportSeries = PassportSeriesTextBox.Text.Trim();
                _client.PassportNumber = PassportNumberTextBox.Text.Trim();
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