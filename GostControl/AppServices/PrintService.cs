using GostControl.AppModels;
using System;
using System.Collections.Generic;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GostControl.AppServices
{
    public static class PrintService
    {
        public static void PrintClients(IEnumerable<Client> clients)
        {
            try
            {
                // Создание диалога печати
                var printDialog = new PrintDialog();

                // Устанавливаем горизонтальную ориентацию
                if (printDialog.PrintTicket == null)
                {
                    printDialog.PrintTicket = printDialog.PrintQueue?.DefaultPrintTicket;
                }

                if (printDialog.PrintTicket != null)
                {
                    printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                }

                // Установка принтера по умолчанию
                var localPrintServer = new LocalPrintServer();
                var defaultPrintQueue = localPrintServer.DefaultPrintQueue;
                printDialog.PrintQueue = defaultPrintQueue;

                if (printDialog.ShowDialog() == true)
                {
                    // Создание документа для печати
                    var document = CreatePrintDocument(clients);

                    // Печать
                    printDialog.PrintDocument(document.DocumentPaginator, "Список клиентов");

                    MessageBox.Show("Документ отправлен на печать",
                        "Печать", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка печати: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static FixedDocument CreatePrintDocument(IEnumerable<Client> clients)
        {
            var document = new FixedDocument();

            // Создание страницы
            var pageContent = new PageContent();
            var fixedPage = new FixedPage();

            // Горизонтальный A4
            fixedPage.Width = 11.69 * 96;  // Высота в дюймах * 96 DPI
            fixedPage.Height = 8.27 * 96;  // Ширина в дюймах * 96 DPI

            // Основной контейнер
            var grid = new Grid
            {
                Width = fixedPage.Width,
                Height = fixedPage.Height,
                Background = Brushes.White
            };

            // Строки Grid
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(70) });     // Заголовок
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });     // Дата и время
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Таблица
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });     // Итог

            // Заголовок
            var titleBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(30, 10, 30, 5)
            };

            var titleText = new TextBlock
            {
                Text = "СПИСОК КЛИЕНТОВ",
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(15, 8, 15, 8)
            };

            titleBorder.Child = titleText;
            Grid.SetRow(titleBorder, 0);
            grid.Children.Add(titleBorder);

            // Дата и время печати
            var dateText = new TextBlock
            {
                Text = $"Дата печати: {DateTime.Now:dd.MM.yyyy HH:mm}",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 30, 0)
            };

            Grid.SetRow(dateText, 1);
            grid.Children.Add(dateText);

            // Grid для таблицы
            var dataGrid = new Grid
            {
                Margin = new Thickness(30, 0, 30, 0)
            };

            // Колонки для данных
            for (int i = 0; i < 10; i++)
            {
                if (i == 0)
                    dataGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                else if (i == 6 || i == 7)
                    dataGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
                else if (i == 8 || i == 9)
                    dataGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });
                else
                    dataGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Заголовки таблицы
            string[] headers = {
                "ID", "Фамилия", "Имя", "Отчество", "Телефон",
                "Email", "Дата рождения", "Дата регистр.", "Серия", "Номер"
            };

            // Добавляем строку для заголовков
            dataGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });

            // Создаем заголовки
            for (int i = 0; i < headers.Length; i++)
            {
                var headerBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(225, 245, 254)),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0.5),
                    Padding = new Thickness(4, 2, 4, 2)
                };

                var headerText = new TextBlock
                {
                    Text = headers[i],
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center
                };

                headerBorder.Child = headerText;

                // Устанавливаем позицию
                Grid.SetRow(headerBorder, 0);
                Grid.SetColumn(headerBorder, i);

                dataGrid.Children.Add(headerBorder);
            }

            // Добавление данных
            int dataRow = 1;
            foreach (var client in clients)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(28) });

                AddTableCell(dataGrid, dataRow, 0, client.ClientID.ToString(), true);
                AddTableCell(dataGrid, dataRow, 1, client.LastName ?? "");
                AddTableCell(dataGrid, dataRow, 2, client.FirstName ?? "");
                AddTableCell(dataGrid, dataRow, 3, client.MiddleName ?? "");
                AddTableCell(dataGrid, dataRow, 4, client.PhoneNumber ?? "");
                AddTableCell(dataGrid, dataRow, 5, client.Email ?? "");
                AddTableCell(dataGrid, dataRow, 6, client.DateOfBirth?.ToString("dd.MM.yyyy") ?? "", true);
                AddTableCell(dataGrid, dataRow, 7, client.RegistrationDate.ToString("dd.MM.yyyy"), true);
                AddTableCell(dataGrid, dataRow, 8, client.PassportSeries ?? "", true);
                AddTableCell(dataGrid, dataRow, 9, client.PassportNumber ?? "", true);

                dataRow++;
            }

            Grid.SetRow(dataGrid, 2);
            grid.Children.Add(dataGrid);

            // Итоговая строка
            int count = 0;
            foreach (var _ in clients) count++;

            var summaryBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 1, 0, 0),
                Margin = new Thickness(30, 5, 30, 5),
                CornerRadius = new CornerRadius(0, 0, 4, 4)
            };

            var summaryGrid = new Grid();
            summaryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            summaryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

            var summaryLabel = new TextBlock
            {
                Text = "Всего клиентов:",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var summaryValue = new TextBlock
            {
                Text = count.ToString(),
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(summaryLabel, 0);
            Grid.SetColumn(summaryValue, 1);
            summaryGrid.Children.Add(summaryLabel);
            summaryGrid.Children.Add(summaryValue);
            summaryBorder.Child = summaryGrid;

            Grid.SetRow(summaryBorder, 3);
            grid.Children.Add(summaryBorder);

            // Добавляем Grid на FixedPage
            fixedPage.Children.Add(grid);

            // Добавляем FixedPage в документ
            pageContent.Child = fixedPage;
            document.Pages.Add(pageContent);

            return document;
        }

        private static void AddTableCell(Grid grid, int row, int column, string text, bool centerAlign = false)
        {
            var cellText = new TextBlock
            {
                Text = Truncate(text, 25),
                FontSize = 10,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 2, 4, 2),
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = centerAlign ? TextAlignment.Center : TextAlignment.Left
            };

            var cellBorder = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0.5, 0.5),
                Child = cellText
            };

            Grid.SetRow(cellBorder, row);
            Grid.SetColumn(cellBorder, column);
            grid.Children.Add(cellBorder);
        }

        private static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }
    }
}