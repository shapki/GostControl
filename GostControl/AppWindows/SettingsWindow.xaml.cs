using System;
using System.Windows;
using System.Windows.Controls;
using GostControl.AppServices;

namespace GostControl.AppWindows
{
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _settings;

        public SettingsWindow()
        {
            InitializeComponent();
            _settings = AppSettings.Load();
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                AutoRefreshCheckBox.IsChecked = _settings.AutoRefreshEnabled;
                ConfirmDeleteCheckBox.IsChecked = _settings.ConfirmDeleteEnabled;
                DoubleClickEditCheckBox.IsChecked = _settings.DoubleClickEditEnabled;
                SoundNotificationsCheckBox.IsChecked = _settings.SoundNotificationsEnabled;
                EmailNotificationsCheckBox.IsChecked = _settings.EmailNotificationsEnabled;

                switch (_settings.PageSize)
                {
                    case 10:
                        PageSizeComboBox.SelectedIndex = 0;
                        break;
                    case 25:
                        PageSizeComboBox.SelectedIndex = 1;
                        break;
                    case 50:
                        PageSizeComboBox.SelectedIndex = 2;
                        break;
                    case 100:
                        PageSizeComboBox.SelectedIndex = 3;
                        break;
                    case -1:
                        PageSizeComboBox.SelectedIndex = 4;
                        break;
                    default:
                        PageSizeComboBox.SelectedIndex = 1;
                        break;
                }

                SetSortByComboBox(_settings.SortBy);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetSortByComboBox(string sortBy)
        {
            for (int i = 0; i < SortByComboBox.Items.Count; i++)
            {
                if (SortByComboBox.Items[i] is ComboBoxItem item &&
                    item.Content.ToString() == sortBy)
                {
                    SortByComboBox.SelectedIndex = i;
                    return;
                }
            }

            SortByComboBox.SelectedIndex = 1;
        }

        private void SaveSettings()
        {
            try
            {
                // Сохранение значений в объект настроек
                _settings.AutoRefreshEnabled = AutoRefreshCheckBox.IsChecked == true;
                _settings.ConfirmDeleteEnabled = ConfirmDeleteCheckBox.IsChecked == true;
                _settings.DoubleClickEditEnabled = DoubleClickEditCheckBox.IsChecked == true;
                _settings.SoundNotificationsEnabled = SoundNotificationsCheckBox.IsChecked == true;
                _settings.EmailNotificationsEnabled = EmailNotificationsCheckBox.IsChecked == true;
                _settings.PageSize = GetPageSizeFromComboBox();
                _settings.SortBy = GetSortByFromComboBox();

                // Сохранение в файл
                if (_settings.Save())
                {
                    MessageBox.Show("Настройки успешно сохранены!", "Сохранено",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetPageSizeFromComboBox()
        {
            if (PageSizeComboBox.SelectedItem is ComboBoxItem pageSizeItem)
            {
                var pageSizeText = pageSizeItem.Content.ToString();

                if (pageSizeText == "10") return 10;
                if (pageSizeText == "25") return 25;
                if (pageSizeText == "50") return 50;
                if (pageSizeText == "100") return 100;
                if (pageSizeText == "Все") return -1;
            }

            return 25;
        }

        private string GetSortByFromComboBox()
        {
            if (SortByComboBox.SelectedItem is ComboBoxItem sortByItem)
            {
                return sortByItem.Content.ToString();
            }

            return "По фамилии (А-Я)";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            DialogResult = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null)
            {
                DialogResult = false;
            }
            base.OnClosing(e);
        }
    }
}