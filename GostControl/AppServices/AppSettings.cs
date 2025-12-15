using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace GostControl.AppServices
{
    public class AppSettings
    {
        public bool AutoRefreshEnabled { get; set; } = false;
        public bool ConfirmDeleteEnabled { get; set; } = true;
        public bool DoubleClickEditEnabled { get; set; } = true;
        public bool SoundNotificationsEnabled { get; set; } = false;
        public bool EmailNotificationsEnabled { get; set; } = false;
        public int PageSize { get; set; } = 25;
        public string SortBy { get; set; } = "По фамилии (А-Я)";

        // Сохранение в папке приложения
        private static string SettingsPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки настроек: {ex.Message}");
            }

            return new AppSettings();
        }

        public bool Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(SettingsPath, json);
                return true;
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                return false;
            }
        }
    }
}