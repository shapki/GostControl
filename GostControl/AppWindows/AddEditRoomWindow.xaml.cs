using GostControl.AppModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GostControl.AppWindows
{
    public partial class AddEditRoomWindow : Window
    {
        public Room Room { get; private set; }
        public List<RoomCategory> Categories { get; private set; }

        public AddEditRoomWindow(Room room, List<RoomCategory> categories, bool isNew)
        {
            InitializeComponent();

            Room = room;
            Categories = categories;
            DataContext = this;
            TitleTextBlock.Text = isNew ? "Добавление нового номера" : "Редактирование номера";
            this.Title = isNew ? "Добавление номера" : "Редактирование номера";
            InitializeFloorComboBox();
            CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
            UpdateCategoryInfo();
        }

        private void InitializeFloorComboBox()
        {
            // Установка выбранного этаж
            foreach (ComboBoxItem item in FloorComboBox.Items)
            {
                if (item.Content.ToString() == Room.Floor.ToString())
                {
                    FloorComboBox.SelectedItem = item;
                    break;
                }
            }

            // Если этаж не найден, выбор 1го
            if (FloorComboBox.SelectedItem == null && FloorComboBox.Items.Count > 0)
            {
                FloorComboBox.SelectedIndex = 0;
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCategoryInfo();
        }

        private void UpdateCategoryInfo()
        {
            if (CategoryComboBox.SelectedItem is RoomCategory selectedCategory)
            {
                CategoryInfoText.Text =
                    $"Название: {selectedCategory.CategoryName}\n" +
                    $"Описание: {selectedCategory.Description}\n" +
                    $"Цена за день: {selectedCategory.BasePrice:C}\n" +
                    $"Макс. вместимость: {selectedCategory.MaxCapacity} чел.";
            }
            else
            {
                CategoryInfoText.Text = "Выберите категорию номера";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(Room.RoomNumber))
            {
                MessageBox.Show("Введите номер комнаты", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                RoomNumberTextBox.Focus();
                return;
            }

            if (Room.CategoryID <= 0)
            {
                MessageBox.Show("Выберите категорию номера", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                CategoryComboBox.Focus();
                return;
            }

            if (Room.Floor <= 0)
            {
                MessageBox.Show("Выберите этаж", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                FloorComboBox.Focus();
                return;
            }

            // Обновление категории из выбранной
            if (CategoryComboBox.SelectedItem is RoomCategory selectedCategory)
            {
                Room.Category = selectedCategory;
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