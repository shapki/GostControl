using GostControl.AppModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace GostControl.AppServices
{
    public static class ExcelExportService
    {
        public static bool ExportClientsToCSV(IEnumerable<Client> clients, string fileName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = $"Клиенты_{DateTime.Now:yyyy-MM-dd_HH-mm}.csv";
                }

                // Путь к папке Загрузки
                string downloadsPath = GetDownloadsFolderPath();
                var filePath = Path.Combine(downloadsPath, fileName);

                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // UTF-8 BOM для корректного отображения кириллицы в Excel
                    writer.Write('\uFEFF');

                    // Заголовки
                    writer.WriteLine("ID;Фамилия;Имя;Отчество;Телефон;Email;Дата рождения;Дата регистрации;Серия паспорта;Номер паспорта");

                    // Данные
                    int count = 0;
                    foreach (var client in clients)
                    {
                        writer.WriteLine(
                            $"{client.ClientID};" +
                            $"{EscapeCsv(client.LastName)};" +
                            $"{EscapeCsv(client.FirstName)};" +
                            $"{EscapeCsv(client.MiddleName)};" +
                            $"{EscapeCsv(client.PhoneNumber)};" +
                            $"{EscapeCsv(client.Email)};" +
                            $"{client.DateOfBirth?.ToString("dd.MM.yyyy")};" +
                            $"{client.RegistrationDate:dd.MM.yyyy};" +
                            $"{EscapeCsv(client.PassportSeries)};" +
                            $"{EscapeCsv(client.PassportNumber)}"
                        );
                        count++;
                    }

                    // Итоговая строка
                    writer.WriteLine();
                    writer.WriteLine($"Всего клиентов:;{count}");
                }

                // Показ сообщения об успехе
                MessageBox.Show($"Файл успешно экспортирован в CSV:\n{filePath}\n\n" +
                               "Файл сохранен в папке Загрузки.\n" +
                               "CSV файл можно открыть в Microsoft Excel.",
                    "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);

                // Открыть папку с файлом
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"/select,\"{filePath}\""
                    });
                }
                catch
                {
                    // Если не удалось открыть папку, попытка открыть файл
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                    }
                    catch
                    {
                        // Игнор
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в CSV: {ex.Message}\n\n" +
                               "Проверьте доступ к папке Загрузки.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Получение пути к папке Загрузки
        private static string GetDownloadsFolderPath()
        {
            try
            {
                // Для Windows 10 и выше
                string downloadsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads");

                // Проверка существования папки
                if (!Directory.Exists(downloadsPath))
                {
                    // Альт способ
                    downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }

                return downloadsPath;
            }
            catch
            {
                // В случае ошибки использовать рабочий стол
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // Нужно ли экранировать
            if (value.Contains(";") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r") || value.Contains(","))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        public static bool ExportClients(IEnumerable<Client> clients)
        {
            return ExportClientsToCSV(clients);
        }
    }
}