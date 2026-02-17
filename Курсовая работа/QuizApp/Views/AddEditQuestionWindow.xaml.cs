using System.Collections.Generic;
using System.Linq;
using System.Windows;
using QuizApp.Models;

namespace QuizApp.Views
{
    public partial class AddEditQuestionWindow : Window
    {
        private readonly Question _question;
        private List<OptionWrapper> _options = new List<OptionWrapper>();

        // Вспомогательный класс для отображения в списках
        private class OptionWrapper
        {
            public string Text { get; set; }
        }

        public AddEditQuestionWindow(Question question)
        {
            InitializeComponent();
            _question = question;

            QuestionNameTextBox.Text = _question.Name;
            QuestionTextTextBox.Text = _question.Text;

            // Загружаем варианты
            foreach (var opt in _question.Options)
                _options.Add(new OptionWrapper { Text = opt });

            OptionsListBox.ItemsSource = _options;
            UpdateCorrectCombo();
        }

        private void UpdateCorrectCombo()
        {
            CorrectOptionComboBox.ItemsSource = _options;
            if (_options.Count > 0 && _question.CorrectOptionIndex >= 0 && _question.CorrectOptionIndex < _options.Count)
                CorrectOptionComboBox.SelectedIndex = _question.CorrectOptionIndex;
        }

        private void OptionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool isSelected = OptionsListBox.SelectedItem != null;
            EditOptionButton.IsEnabled = isSelected;
            DeleteOptionButton.IsEnabled = isSelected;
        }

        private void AddOption_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog("Введите вариант ответа:", "");
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Answer))
            {
                _options.Add(new OptionWrapper { Text = dialog.Answer.Trim() });
                OptionsListBox.Items.Refresh();
                UpdateCorrectCombo();
            }
        }

        private void EditOption_Click(object sender, RoutedEventArgs e)
        {
            var selected = (OptionWrapper)OptionsListBox.SelectedItem;
            if (selected == null) return;

            var dialog = new InputDialog("Измените вариант ответа:", selected.Text);
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Answer))
            {
                selected.Text = dialog.Answer.Trim();
                OptionsListBox.Items.Refresh();
                UpdateCorrectCombo();
            }
        }

        private void DeleteOption_Click(object sender, RoutedEventArgs e)
        {
            var selected = (OptionWrapper)OptionsListBox.SelectedItem;
            if (selected == null) return;

            int oldIndex = _options.IndexOf(selected);
            _options.Remove(selected);
            OptionsListBox.Items.Refresh();

            // Корректируем индекс правильного ответа, если нужно
            if (_question.CorrectOptionIndex > oldIndex)
                _question.CorrectOptionIndex--;
            else if (_question.CorrectOptionIndex == oldIndex)
                _question.CorrectOptionIndex = -1; // правильный ответ удалён

            UpdateCorrectCombo();
        }

        private void CorrectOptionComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CorrectOptionComboBox.SelectedItem != null)
            {
                _question.CorrectOptionIndex = CorrectOptionComboBox.SelectedIndex;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(QuestionNameTextBox.Text))
            {
                MessageBox.Show("Введите краткое название вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(QuestionTextTextBox.Text))
            {
                MessageBox.Show("Введите текст вопроса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_options.Count < 2)
            {
                MessageBox.Show("Должно быть минимум 2 варианта ответа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_question.CorrectOptionIndex < 0 || _question.CorrectOptionIndex >= _options.Count)
            {
                MessageBox.Show("Выберите правильный ответ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _question.Name = QuestionNameTextBox.Text.Trim();
            _question.Text = QuestionTextTextBox.Text.Trim();
            _question.Options = _options.Select(o => o.Text).ToList();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}