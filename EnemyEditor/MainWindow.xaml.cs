using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Text.Json;

namespace EnemyEditor
{
    public partial class MainWindow : Window
    {
        private List<CEnemyTemplate> enemies = new List<CEnemyTemplate>();
        private ObservableCollection<IconItem> availableIcons = new ObservableCollection<IconItem>();
        private const string IconsFolder = "icons";

        public MainWindow()
        {
            InitializeComponent();
            LoadAvailableIcons();
            SetDefaultValues();
            
        }

        // ------------------------------------------------------------
        // Загрузка иконок из папки
        // ------------------------------------------------------------
        private void LoadAvailableIcons()
        {
            try
            {
                string fullIconsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, IconsFolder);

                if (!Directory.Exists(fullIconsPath))
                {
                    Directory.CreateDirectory(fullIconsPath);
                    MessageBox.Show($"Папка '{IconsFolder}' создана.\nДобавьте в неё PNG-файлы и перезапустите программу.");
                }

                string[] files = Directory.GetFiles(fullIconsPath, "*.png");

                foreach (string file in files)
                {
                    string iconName = Path.GetFileNameWithoutExtension(file);
                    BitmapImage bitmap = LoadImageFromFile(file);
                    if (bitmap != null)
                    {
                        availableIcons.Add(new IconItem(iconName, bitmap));
                    }
                }

                IconsListBox.ItemsSource = availableIcons;

                List<string> iconNames = availableIcons.Select(i => i.Name).ToList();
                IconNameComboBox.ItemsSource = iconNames;

                if (availableIcons.Count > 0)
                {
                    IconNameComboBox.SelectedIndex = 0;
                    IconsListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки иконок: {ex.Message}");
            }
        }

        private BitmapImage LoadImageFromFile(string filePath)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        // ------------------------------------------------------------
        // Значения по умолчанию
        // ------------------------------------------------------------
        private void SetDefaultValues()
        {
            BaseLifeTextBox.Text = "12";
            LifeModifierTextBox.Text = "54";
            BaseGoldTextBox.Text = "15";
            GoldModifierTextBox.Text = "67";
            SpawnChanceTextBox.Text = "13";
            EnemyNameTextBox.Text = "Enemy";
        }


        private void RefreshEnemiesList()
        {
            EnemiesListBox.ItemsSource = null;
            EnemiesListBox.ItemsSource = enemies;
        }

        // ------------------------------------------------------------
        // Выбор противника в списке -> загрузка его данных
        // ------------------------------------------------------------
        private void EnemiesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CEnemyTemplate selected = EnemiesListBox.SelectedItem as CEnemyTemplate;
            if (selected == null) return;

            EnemyNameTextBox.Text = selected.GetName();
            BaseLifeTextBox.Text = selected.GetBaseLife().ToString();
            LifeModifierTextBox.Text = selected.GetLifeModifier().ToString("0.##");
            BaseGoldTextBox.Text = selected.GetBaseGold().ToString();
            GoldModifierTextBox.Text = selected.GetGoldModifier().ToString("0.##");
            SpawnChanceTextBox.Text = (selected.GetSpawnChance() * 100).ToString("0");

            string iconName = selected.GetIconName();
            IconItem icon = availableIcons.FirstOrDefault(i => i.Name == iconName);
            if (icon != null)
            {
                EnemyIconImage.Source = icon.Image;
                EnemyIconImage.Visibility = Visibility.Visible;
                IconPlaceholderText.Visibility = Visibility.Collapsed;

                IconNameComboBox.SelectedItem = iconName;
                IconsListBox.SelectedItem = icon;
            }
            else
            {
                EnemyIconImage.Visibility = Visibility.Collapsed;
                IconPlaceholderText.Visibility = Visibility.Visible;
            }
        }

        // ------------------------------------------------------------
        // Изменение любого текстового поля -> обновление текущего противника
        // ------------------------------------------------------------
        private void Field_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CEnemyTemplate current = EnemiesListBox.SelectedItem as CEnemyTemplate;
            if (current == null) return;

            try
            {
                if (sender == BaseLifeTextBox)
                    current.SetBaseLife(int.Parse(BaseLifeTextBox.Text));
                else if (sender == LifeModifierTextBox)
                    current.SetLifeModifier(double.Parse(LifeModifierTextBox.Text));
                else if (sender == BaseGoldTextBox)
                    current.SetBaseGold(int.Parse(BaseGoldTextBox.Text));
                else if (sender == GoldModifierTextBox)
                    current.SetGoldModifier(double.Parse(GoldModifierTextBox.Text));
                else if (sender == SpawnChanceTextBox)
                    current.SetSpawnChance(double.Parse(SpawnChanceTextBox.Text) / 100.0);
                else if (sender == EnemyNameTextBox)
                {
                    current.SetName(EnemyNameTextBox.Text);
                    // Обновляем отображение имени в ListBox
                    EnemiesListBox.Items.Refresh();
                }
            }
            catch
            {
                // Игнорируем ошибки парсинга при незавершённом вводе
            }
        }

        // ------------------------------------------------------------
        // Выбор иконки из выпадающего списка
        // ------------------------------------------------------------
        private void IconNameComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string iconName = IconNameComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(iconName)) return;

            IconItem icon = availableIcons.FirstOrDefault(i => i.Name == iconName);
            if (icon != null)
            {
                EnemyIconImage.Source = icon.Image;
                EnemyIconImage.Visibility = Visibility.Visible;
                IconPlaceholderText.Visibility = Visibility.Collapsed;
                IconsListBox.SelectedItem = icon;

                // Обновляем иконку у текущего противника
                CEnemyTemplate current = EnemiesListBox.SelectedItem as CEnemyTemplate;
                if (current != null)
                {
                    current.SetIconName(iconName);
                }
            }
        }

        // ------------------------------------------------------------
        // Выбор иконки из правого списка
        // ------------------------------------------------------------
        private void IconsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            IconItem selected = IconsListBox.SelectedItem as IconItem;
            if (selected != null)
            {
                IconNameComboBox.SelectedItem = selected.Name;
            }
        }

        // ------------------------------------------------------------
        // Добавление нового противника
        // ------------------------------------------------------------
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (IconNameComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите иконку для противника.");
                return;
            }

            if (string.IsNullOrWhiteSpace(EnemyNameTextBox.Text))
            {
                MessageBox.Show("Введите имя противника.");
                return;
            }

            try
            {
                string name = EnemyNameTextBox.Text;
                string iconName = IconNameComboBox.SelectedItem.ToString();
                int baseLife = int.Parse(BaseLifeTextBox.Text);
                double lifeModifier = double.Parse(LifeModifierTextBox.Text);
                int baseGold = int.Parse(BaseGoldTextBox.Text);
                double goldModifier = double.Parse(GoldModifierTextBox.Text);
                double spawnChance = double.Parse(SpawnChanceTextBox.Text) / 100.0;

                CEnemyTemplate newEnemy = new CEnemyTemplate(
                    name, iconName, baseLife, lifeModifier,
                    baseGold, goldModifier, spawnChance);

                enemies.Add(newEnemy);
                RefreshEnemiesList();
                EnemiesListBox.SelectedItem = newEnemy; // выделяем нового
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
            }
        }

        // ------------------------------------------------------------
        // Удаление выбранного противника
        // ------------------------------------------------------------
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            CEnemyTemplate selected = EnemiesListBox.SelectedItem as CEnemyTemplate;
            if (selected != null)
            {
                enemies.Remove(selected);
                RefreshEnemiesList();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Выберите противника для удаления.");
            }
        }

        // ------------------------------------------------------------
        // Очистка полей
        // ------------------------------------------------------------
        private void ClearFields()
        {
            EnemyNameTextBox.Text = "";
            BaseLifeTextBox.Text = "12";
            LifeModifierTextBox.Text = "54";
            BaseGoldTextBox.Text = "15";
            GoldModifierTextBox.Text = "67";
            SpawnChanceTextBox.Text = "13";

            EnemyIconImage.Visibility = Visibility.Collapsed;
            IconPlaceholderText.Visibility = Visibility.Visible;
        }

        // ------------------------------------------------------------
        // Сохранение списка в JSON
        // ------------------------------------------------------------
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "JSON files (*.json)|*.json";
            saveDialog.DefaultExt = "json";
            saveDialog.FileName = "enemies.json";

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    string jsonString = JsonSerializer.Serialize(enemies, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(saveDialog.FileName, jsonString);
                    MessageBox.Show($"Сохранено: {saveDialog.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                }
            }
        }

        // ------------------------------------------------------------
        // Загрузка списка из JSON
        // ------------------------------------------------------------
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "JSON files (*.json)|*.json";

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    string jsonFromFile = File.ReadAllText(openDialog.FileName);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    enemies = JsonSerializer.Deserialize<List<CEnemyTemplate>>(jsonFromFile, options);
                    RefreshEnemiesList();
                    ClearFields();
                    MessageBox.Show($"Загружено: {openDialog.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }
            }
        }
    }


    public class IconItem
    {
        public string Name { get; set; }
        public BitmapImage Image { get; set; }

        public IconItem(string name, BitmapImage image)
        {
            Name = name;
            Image = image;
        }
    }
}